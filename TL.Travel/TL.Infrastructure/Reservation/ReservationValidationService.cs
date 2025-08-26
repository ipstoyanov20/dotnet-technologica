using System;
using System.Linq;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;

namespace TL.Travel.Infrastructure.Reservations
{
    public class ReservationValidationService
    {
        private readonly BaseTLTravelDbContext _dbContext;

        public ReservationValidationService(BaseTLTravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ValidateReservationData(ReservationUM body, int reservationId = 0)
        {
            ValidateRequiredFields(body);
            ValidateRoomItems(body);
            ValidatePaymentItems(body);
            ValidateForeignKeys(body);
            ValidateDates(body);
            ValidateRoomCapacity(body);
            ValidateGuestCounts(body);
            ValidatePaymentAmounts(body);
            ValidateBusinessRules(body);
            ValidateRoomAvailability(body, reservationId);

            if (reservationId > 0)
            {
                ValidatePaymentUpdateRestrictions(reservationId);
            }
        }

        private void ValidateRequiredFields(ReservationUM body)
        {
            if (!body.DateFrom.HasValue)
                throw new ArgumentException("DateFrom is required");

            if (!body.DateTo.HasValue)
                throw new ArgumentException("DateTo is required");

            if (!body.ClientId.HasValue)
                throw new ArgumentException("ClientId is required");

            if (!body.OperatorId.HasValue)
                throw new ArgumentException("OperatorId is required");

            if (!body.HotelId.HasValue)
                throw new ArgumentException("HotelId is required");

            if (body.RoomItems == null || !body.RoomItems.Any())
            {
                throw new ArgumentException("At least one room must be selected");
            }

            if (body.PaymentItems == null || !body.PaymentItems.Any())
            {
                throw new ArgumentException("At least one payment must be specified");
            }
        }

        private void ValidateRoomItems(ReservationUM body)
        {
            if (body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    if (!roomItem.HotelRoomId.HasValue)
                        throw new ArgumentException("Hotel room is required for each room item");

                    if (!roomItem.FeedingTypeId.HasValue)
                        throw new ArgumentException("Feeding type is required for each room item");

                    if (!roomItem.RoomCount.HasValue || roomItem.RoomCount.Value < 1)
                        throw new ArgumentException("Room count must be at least 1");

                    if (!roomItem.Adults.HasValue || roomItem.Adults.Value < 1)
                        throw new ArgumentException("At least one adult is required per room");

                    if (!roomItem.Children.HasValue || roomItem.Children.Value < 0)
                        throw new ArgumentException("Children count cannot be negative");

                    if (!roomItem.Babies.HasValue || roomItem.Babies.Value < 0)
                        throw new ArgumentException("Babies count cannot be negative");

                    if (!roomItem.PricePerRoom.HasValue || roomItem.PricePerRoom.Value <= 0)
                        throw new ArgumentException("Price per room must be greater than zero");
                }
            }
        }

        private void ValidatePaymentItems(ReservationUM body)
        {
            if (body.PaymentItems != null && body.PaymentItems.Any())
            {
                foreach (var paymentItem in body.PaymentItems)
                {
                    if (!paymentItem.PaymentTypeId.HasValue)
                        throw new ArgumentException("Payment type is required for each payment item");

                    if (!paymentItem.PaymentChannelId.HasValue)
                        throw new ArgumentException("Payment channel is required for each payment item");

                    if (!paymentItem.Amount.HasValue || paymentItem.Amount.Value <= 0)
                        throw new ArgumentException("Payment amount must be greater than zero");

                    if (!paymentItem.DueDate.HasValue)
                        throw new ArgumentException("Due date is required for each payment item");

                    if (paymentItem.DueDate.HasValue && body.DateFrom.HasValue)
                    {
                        if (paymentItem.DueDate.Value >= body.DateFrom.Value)
                            throw new ArgumentException("Payment due date must be earlier than reservation start date");
                    }
                }
            }
        }

        private void ValidateForeignKeys(ReservationUM body)
        {
            if (body.ClientId.HasValue && !_dbContext.Clients.Any(c => c.Id == body.ClientId.Value && c.IsActive))
                throw new ArgumentException("Invalid or inactive Client");

            if (body.OperatorId.HasValue && !_dbContext.Agents.Any(o => o.Id == body.OperatorId.Value && o.IsActive))
                throw new ArgumentException("Invalid or inactive Operator");

            if (body.HotelId.HasValue && !_dbContext.Hotels.Any(h => h.Id == body.HotelId.Value && h.IsActive))
                throw new ArgumentException("Invalid or inactive Hotel");

            if (body.ReservationStatusId.HasValue && !_dbContext.ReservationStatuses.Any(rs => rs.Id == body.ReservationStatusId.Value && rs.IsActive))
                throw new ArgumentException("Invalid or inactive Reservation Status");

            if (body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    if (roomItem.HotelRoomId.HasValue && !_dbContext.HotelRooms.Any(hr => hr.Id == roomItem.HotelRoomId.Value && hr.IsActive))
                        throw new ArgumentException("Invalid or inactive Hotel Room");

                    if (roomItem.FeedingTypeId.HasValue && !_dbContext.FeedingTypes.Any(ft => ft.Id == roomItem.FeedingTypeId.Value && ft.IsActive))
                        throw new ArgumentException("Invalid or inactive Feeding Type");
                }
            }

            if (body.PaymentItems != null && body.PaymentItems.Any())
            {
                foreach (var paymentItem in body.PaymentItems)
                {
                    if (paymentItem.PaymentTypeId.HasValue && !_dbContext.PaymentTypes.Any(pt => pt.Id == paymentItem.PaymentTypeId.Value && pt.IsActive))
                        throw new ArgumentException("Invalid or inactive Payment Type");

                    if (paymentItem.PaymentChannelId.HasValue && !_dbContext.PaymentChannels.Any(pc => pc.Id == paymentItem.PaymentChannelId.Value && pc.IsActive))
                        throw new ArgumentException("Invalid or inactive Payment Channel");
                }
            }
        }

        private void ValidateDates(ReservationUM body)
        {
            if (body.DateFrom.HasValue && body.DateTo.HasValue)
            {
                if (body.DateFrom.Value >= body.DateTo.Value)
                    throw new ArgumentException("DateFrom must be earlier than DateTo");

                if (body.DateFrom.Value < DateTime.Today)
                    throw new ArgumentException("DateFrom cannot be in the past");
            }
        }

        private void ValidateRoomCapacity(ReservationUM body)
        {
            if (body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    if (!roomItem.HotelRoomId.HasValue) continue;

                    var hotelRoom = _dbContext.HotelRooms.FirstOrDefault(hr => hr.Id == roomItem.HotelRoomId.Value && hr.IsActive);
                    if (hotelRoom == null)
                        throw new ArgumentException("Hotel room not found or inactive");

                    if (roomItem.Adults.Value > hotelRoom.MaxAdults)
                        throw new ArgumentException($"Number of adults ({roomItem.Adults.Value}) exceeds room capacity ({hotelRoom.MaxAdults})");

                    if (roomItem.Children.Value > hotelRoom.MaxChildren)
                        throw new ArgumentException($"Number of children ({roomItem.Children.Value}) exceeds room capacity ({hotelRoom.MaxChildren})");

                    if (roomItem.Babies.Value > hotelRoom.MaxBabies)
                        throw new ArgumentException($"Number of babies ({roomItem.Babies.Value}) exceeds room capacity ({hotelRoom.MaxBabies})");
                }
            }
        }

        private void ValidateGuestCounts(ReservationUM body)
        {
            if (body.Adults.HasValue && body.Adults.Value < 1)
                throw new ArgumentException("At least one adult is required");

            if (body.Children.HasValue && body.Children.Value < 0)
                throw new ArgumentException("Children count cannot be negative");

            if (body.Babies.HasValue && body.Babies.Value < 0)
                throw new ArgumentException("Babies count cannot be negative");

            if (body.Adults.HasValue && body.Children.HasValue && body.Babies.HasValue)
            {
                var totalGuests = body.Adults.Value + body.Children.Value + body.Babies.Value;
                if (totalGuests > ReservationConstants.MAX_GUESTS_PER_RESERVATION)
                    throw new ArgumentException("Total number of guests exceeds maximum allowed");
            }
        }

        private void ValidatePaymentAmounts(ReservationUM body)
        {
            if (body.PaymentAmount.HasValue && body.PaymentAmount.Value < 0)
                throw new ArgumentException("PaymentAmount cannot be negative");

            var totalPayment = body.PaymentItems?.Sum(pi => pi.Amount ?? 0) ?? 0;
            var totalPrice = body.TotalPrice ?? 0;

            if (totalPayment > totalPrice)
                throw new ArgumentException("Total payment amount cannot exceed total price");
        }

        private void ValidateBusinessRules(ReservationUM body)
        {
            if (body.HotelId.HasValue && body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    if (roomItem.HotelRoomId.HasValue)
                    {
                        var roomBelongsToHotel = _dbContext.HotelRooms.Any(hr => hr.Id == roomItem.HotelRoomId.Value && hr.HotelId == body.HotelId.Value);
                        if (!roomBelongsToHotel)
                            throw new ArgumentException("Selected room does not belong to the specified hotel");
                    }
                }
            }

            if (body.HotelId.HasValue)
            {
                var hotel = _dbContext.Hotels.FirstOrDefault(h => h.Id == body.HotelId.Value);
                if (hotel?.IsTemporaryClosed == true)
                    throw new ArgumentException("Hotel is temporarily closed and not accepting reservations");
            }
        }

        private void ValidateRoomAvailability(ReservationUM body, int reservationId = 0)
        {
            if (!body.DateFrom.HasValue || !body.DateTo.HasValue)
                return;

            if (body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    if (!roomItem.HotelRoomId.HasValue) continue;

                    var conflictingReservations = from r in _dbContext.Reservations
                                                  join rr in _dbContext.ReservationRooms on r.Id equals rr.ReservationId
                                                  where r.IsActive
                                                  && rr.HotelRoomId == roomItem.HotelRoomId.Value
                                                  && r.Id != reservationId
                                                  && r.ReservationStatusId != ReservationConstants.STATUS_CANCELED
                                                  && (body.DateFrom.Value < r.DateTo && body.DateTo.Value > r.DateFrom)
                                                  select r;

                    if (conflictingReservations.Any())
                    {
                        var conflictingReservation = conflictingReservations.First();
                        throw new ArgumentException($"Room is already booked from {conflictingReservation.DateFrom:yyyy-MM-dd} to {conflictingReservation.DateTo:yyyy-MM-dd}");
                    }
                }
            }
        }

        private void ValidatePaymentUpdateRestrictions(int reservationId)
        {
            var existingPayments = _dbContext.ReservationPayments.Where(rp => rp.ReservationId == reservationId && rp.IsActive).ToList();
            if (existingPayments.Any(ep => ep.IsPaid))
            {
                throw new ArgumentException("Cannot update reservation when some payments are already marked as paid");
            }
        }
    }
}
