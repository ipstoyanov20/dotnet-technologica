using System;
using System.Linq;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;

namespace TL.Travel.Infrastructure.Reservations
{
    public class ReservationManagementService
    {
        private readonly BaseTLTravelDbContext _dbContext;
        private readonly ReservationValidationService _validationService;
        private readonly ReservationPricingService _pricingService;
        private readonly ReservationStatusManager _statusManager;

        public ReservationManagementService(
            BaseTLTravelDbContext dbContext,
            ReservationValidationService validationService,
            ReservationPricingService pricingService,
            ReservationStatusManager statusManager)
        {
            _dbContext = dbContext;
            _validationService = validationService;
            _pricingService = pricingService;
            _statusManager = statusManager;
        }

        public ReservationVM AddEdit(ReservationUM body, int id = 0)
        {
            _pricingService.ValidateAndCalculateRoomPrices(body);
            _pricingService.ValidatePaymentItems(body);
            _validationService.ValidateReservationData(body, id);

            if (id > 0)
            {
                return UpdateExistingReservation(body, id);
            }
            else
            {
                return CreateNewReservation(body);
            }
        }

        private ReservationVM UpdateExistingReservation(ReservationUM body, int id)
        {
            var reservation = _dbContext.Reservations.FirstOrDefault(r => r.Id == id && r.IsActive);

            if (reservation == null)
                throw new ArgumentException("No such reservation");

            UpdateReservationEntity(reservation, body);
            UpdateReservationRoomsWithComparison(reservation.Id, body);
            UpdateReservationPaymentsWithComparison(reservation.Id, body);

            _dbContext.SaveChanges();
            _pricingService.UpdateReservationTotalPrice(reservation.Id);

            var totalPayment = _pricingService.CalculateTotalPaymentFromReservation(reservation.Id);
            if (totalPayment > reservation.TotalPrice)
                throw new ArgumentException("Total payment amount cannot exceed total price");

            var determinedStatusId = _statusManager.DetermineReservationStatusFromDatabase(reservation.Id);
            reservation.ReservationStatusId = determinedStatusId;

            _dbContext.SaveChanges();
            return GetReservationById(reservation.Id);
        }

        private ReservationVM CreateNewReservation(ReservationUM body)
        {
            var reservation = new Reservation
            {
                DateFrom = body.DateFrom.Value,
                DateTo = body.DateTo.Value,
                CustomerNotes = body.CustomerNotes,
                TotalPrice = body.TotalPrice ?? 0,
                IsActive = body.IsActive,
                ClientId = body.ClientId.Value,
                OperatorId = body.OperatorId.Value,
                HotelId = body.HotelId.Value,
                ReservationStatusId = ReservationConstants.STATUS_UNPAID
            };
            _dbContext.Reservations.Add(reservation);
            _dbContext.SaveChanges();

            CreateReservationRooms(reservation.Id, body);
            CreateReservationPayments(reservation.Id, body);
            _dbContext.SaveChanges();

            _pricingService.UpdateReservationTotalPrice(reservation.Id);

            var determinedStatusId = _statusManager.DetermineReservationStatus(body, reservation.TotalPrice);
            reservation.ReservationStatusId = determinedStatusId;

            _dbContext.SaveChanges();
            return GetReservationById(reservation.Id);
        }

        private void CreateReservationRooms(int reservationId, ReservationUM body)
        {
            if (body.RoomItems != null && body.RoomItems.Any())
            {
                foreach (var roomItem in body.RoomItems)
                {
                    var reservationRoom = new ReservationRoom
                    {
                        ReservationId = reservationId,
                        Adults = roomItem.Adults.Value,
                        Children = roomItem.Children.Value,
                        Babies = roomItem.Babies.Value,
                        Price = roomItem.PricePerRoom.Value * roomItem.RoomCount.Value,
                        HotelRoomId = roomItem.HotelRoomId.Value,
                        FeedingTypeId = roomItem.FeedingTypeId.Value
                    };
                    _dbContext.ReservationRooms.Add(reservationRoom);
                }
            }
        }

        private void CreateReservationPayments(int reservationId, ReservationUM body)
        {
            if (body.PaymentItems != null && body.PaymentItems.Any())
            {
                foreach (var paymentItem in body.PaymentItems)
                {
                    var reservationPayment = new ReservationPayment
                    {
                        ReservationId = reservationId,
                        DueDate = paymentItem.DueDate.Value,
                        PaymentTypeId = paymentItem.PaymentTypeId.Value,
                        PaymentChannelId = paymentItem.PaymentChannelId.Value,
                        Amount = paymentItem.Amount.Value,
                        IsPaid = paymentItem.IsPaid,
                        IsActive = paymentItem.IsActive,
                        CreatedBy = "System",
                        CreatedOn = DateTime.UtcNow
                    };
                    _dbContext.ReservationPayments.Add(reservationPayment);
                }
            }
        }

        private void UpdateReservationRoomsWithComparison(int reservationId, ReservationUM body)
        {
            if (body.RoomItems != null && body.RoomItems.Count > 0)
            {
                var requestRoomKeys = body.RoomItems.Select(ri => new
                {
                    HotelRoomId = ri.HotelRoomId.Value,
                    FeedingTypeId = ri.FeedingTypeId.Value,
                    Adults = ri.Adults.Value,
                    Children = ri.Children.Value,
                    Babies = ri.Babies.Value,
                    Price = ri.PricePerRoom.Value * ri.RoomCount.Value
                }).ToList();

                var existingRooms = _dbContext.ReservationRooms
                    .Where(x => x.ReservationId == reservationId)
                    .ToList();

                var dbRoomKeys = existingRooms.Select(e => new
                {
                    e.HotelRoomId,
                    e.FeedingTypeId,
                    e.Adults,
                    e.Children,
                    e.Babies,
                    e.Price
                }).ToList();

                var toBeAdded = requestRoomKeys.Except(dbRoomKeys).ToList();
                foreach (var roomKey in toBeAdded)
                {
                    if (_dbContext.HotelRooms.Any(x => x.Id == roomKey.HotelRoomId && x.IsActive))
                    {
                        _dbContext.ReservationRooms.Add(new ReservationRoom
                        {
                            ReservationId = reservationId,
                            HotelRoomId = roomKey.HotelRoomId,
                            FeedingTypeId = roomKey.FeedingTypeId,
                            Adults = roomKey.Adults,
                            Children = roomKey.Children,
                            Babies = roomKey.Babies,
                            Price = roomKey.Price
                        });
                    }
                }

                var toBeDeleted = dbRoomKeys.Except(requestRoomKeys).ToList();
                foreach (var roomKey in toBeDeleted)
                {
                    var toRemove = existingRooms.FirstOrDefault(x => 
                        x.HotelRoomId == roomKey.HotelRoomId && 
                        x.FeedingTypeId == roomKey.FeedingTypeId &&
                        x.Adults == roomKey.Adults &&
                        x.Children == roomKey.Children &&
                        x.Babies == roomKey.Babies &&
                        x.Price == roomKey.Price);
                    if (toRemove != null)
                        _dbContext.ReservationRooms.Remove(toRemove);
                }
            }
        }

        private void UpdateReservationPaymentsWithComparison(int reservationId, ReservationUM body)
        {
            var existingPayments = _dbContext.ReservationPayments
                .Where(x => x.ReservationId == reservationId && x.IsActive)
                .ToList();

            if (existingPayments.Any(ep => ep.IsPaid))
            {
                throw new ArgumentException("Cannot update payments when some payments are already marked as paid");
            }

            if (body.PaymentItems != null && body.PaymentItems.Count > 0)
            {
                var requestPaymentKeys = body.PaymentItems.Select(pi => new
                {
                    PaymentTypeId = pi.PaymentTypeId.Value,
                    PaymentChannelId = pi.PaymentChannelId.Value,
                    Amount = pi.Amount.Value,
                    DueDate = pi.DueDate.Value,
                    IsPaid = pi.IsPaid,
                    IsActive = pi.IsActive
                }).ToList();

                var dbPaymentKeys = existingPayments.Select(e => new
                {
                    e.PaymentTypeId,
                    e.PaymentChannelId,
                    e.Amount,
                    e.DueDate,
                    e.IsPaid,
                    e.IsActive
                }).ToList();

                var toBeAdded = requestPaymentKeys.Except(dbPaymentKeys).ToList();
                foreach (var paymentKey in toBeAdded)
                {
                    if (_dbContext.PaymentTypes.Any(x => x.Id == paymentKey.PaymentTypeId && x.IsActive))
                    {
                        _dbContext.ReservationPayments.Add(new ReservationPayment
                        {
                            ReservationId = reservationId,
                            PaymentTypeId = paymentKey.PaymentTypeId,
                            PaymentChannelId = paymentKey.PaymentChannelId,
                            Amount = paymentKey.Amount,
                            DueDate = paymentKey.DueDate,
                            IsPaid = paymentKey.IsPaid,
                            IsActive = paymentKey.IsActive,
                            CreatedBy = "System",
                            CreatedOn = DateTime.UtcNow
                        });
                    }
                }

                var toBeDeleted = dbPaymentKeys.Except(requestPaymentKeys).ToList();
                foreach (var paymentKey in toBeDeleted)
                {
                    var toRemove = existingPayments.FirstOrDefault(x => 
                        x.PaymentTypeId == paymentKey.PaymentTypeId && 
                        x.PaymentChannelId == paymentKey.PaymentChannelId &&
                        x.Amount == paymentKey.Amount &&
                        x.DueDate == paymentKey.DueDate &&
                        x.IsPaid == paymentKey.IsPaid &&
                        x.IsActive == paymentKey.IsActive);
                    if (toRemove != null && !toRemove.IsPaid)
                        _dbContext.ReservationPayments.Remove(toRemove);
                }

                var toBeUpdated = dbPaymentKeys.Intersect(requestPaymentKeys).ToList();
                foreach (var paymentKey in toBeUpdated)
                {
                    var toUpdate = existingPayments.FirstOrDefault(x => 
                        x.PaymentTypeId == paymentKey.PaymentTypeId && 
                        x.PaymentChannelId == paymentKey.PaymentChannelId &&
                        x.Amount == paymentKey.Amount &&
                        x.DueDate == paymentKey.DueDate);
                    if (toUpdate != null && !toUpdate.IsPaid)
                    {
                        toUpdate.IsActive = paymentKey.IsActive;
                        toUpdate.UpdatedBy = "System";
                        toUpdate.UpdatedOn = DateTime.UtcNow;
                    }
                }
            }
        }

        private void UpdateReservationEntity(Reservation reservation, ReservationUM body)
        {
            reservation.DateFrom = body.DateFrom.Value;
            reservation.DateTo = body.DateTo.Value;
            reservation.CustomerNotes = body.CustomerNotes;
            reservation.IsActive = body.IsActive;
            reservation.ClientId = body.ClientId.Value;
            reservation.OperatorId = body.OperatorId.Value;
            reservation.HotelId = body.HotelId.Value;
        }

        public void Delete(int id)
        {
            var reservation = _dbContext.Reservations.FirstOrDefault(r => r.Id == id && r.IsActive);
            if (reservation == null)
                throw new ArgumentException("No such reservation");

            reservation.IsActive = false;
            _dbContext.SaveChanges();
        }

        private ReservationVM GetReservationById(int id)
        {
            var reservationData = (from r in _dbContext.Reservations
                                   join c in _dbContext.Clients on r.ClientId equals c.Id
                                   join o in _dbContext.Agents on r.OperatorId equals o.Id
                                   join rs in _dbContext.ReservationStatuses on r.ReservationStatusId equals rs.Id
                                   where r.Id == id && r.IsActive
                                   select new
                                   {
                                       r.Id,
                                       r.DateFrom,
                                       r.DateTo,
                                       r.CustomerNotes,
                                       r.IsActive,
                                       r.ClientId,
                                       ClientName = c.Name,
                                       r.OperatorId,
                                       OperatorName = o.Name,
                                       r.ReservationStatusId,
                                       ReservationStatusName = rs.Name
                                   }).FirstOrDefault();

            if (reservationData == null)
                throw new ArgumentException("Reservation not found");

            return new ReservationVM
            {
                Id = reservationData.Id,
                DateFrom = reservationData.DateFrom,
                DateTo = reservationData.DateTo,
                CustomerNotes = reservationData.CustomerNotes,
                TotalPrice = _pricingService.CalculateTotalPriceFromRooms(reservationData.Id),
                IsActive = reservationData.IsActive,
                ClientId = reservationData.ClientId,
                ClientName = reservationData.ClientName,
                OperatorId = reservationData.OperatorId,
                OperatorName = reservationData.OperatorName,
                ReservationStatusId = reservationData.ReservationStatusId,
                ReservationStatusName = reservationData.ReservationStatusName
            };
        }
    }
}
