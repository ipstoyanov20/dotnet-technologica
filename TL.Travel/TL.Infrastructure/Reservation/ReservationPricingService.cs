using System.Linq;
using System.Collections.Generic;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;
using System;

namespace TL.Travel.Infrastructure.Reservations
{
    public class ReservationPricingService
    {
        private readonly BaseTLTravelDbContext _dbContext;

        public ReservationPricingService(BaseTLTravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public decimal CalculateTotalPriceFromRooms(int reservationId)
        {
            return _dbContext.ReservationRooms
                .Where(rr => rr.ReservationId == reservationId)
                .Sum(rr => rr.Price);
        }

        public decimal CalculateTotalPriceFromRoomItems(List<ReservationRoomItemUM> roomItems)
        {
            if (roomItems == null || !roomItems.Any())
                return 0;

            return roomItems.Sum(item =>
                (item.PricePerRoom ?? 0) * (item.RoomCount ?? 0));
        }

        public decimal CalculateTotalPaymentFromItems(List<ReservationPaymentItemUM> paymentItems)
        {
            if (paymentItems == null || !paymentItems.Any())
                return 0;

            return paymentItems.Sum(item => item.Amount ?? 0);
        }

        public decimal CalculateTotalPaymentFromReservation(int reservationId)
        {
            return _dbContext.ReservationPayments
                .Where(rp => rp.ReservationId == reservationId && rp.IsActive)
                .Sum(rp => rp.Amount);
        }

        public decimal CalculatePriceForRoomType(int hotelRoomId, int feedingTypeId, int nights)
        {
            var hotelRoom = _dbContext.HotelRooms
                .FirstOrDefault(hr => hr.Id == hotelRoomId && hr.IsActive);

            if (hotelRoom == null)
                throw new ArgumentException("Hotel room not found or inactive");

            var feedingTypePrice = GetFeedingTypePrice(hotelRoomId, feedingTypeId);

            return (hotelRoom.Price + feedingTypePrice) * nights;
        }

        private decimal GetFeedingTypePrice(int hotelRoomId, int feedingTypeId)
        {
            return 0;
        }

        public void ValidateAndCalculateRoomPrices(ReservationUM body)
        {
            if (body.RoomItems == null || !body.RoomItems.Any())
            {
                throw new ArgumentException("At least one room must be selected");
            }

            var nights = body.DateFrom.HasValue && body.DateTo.HasValue
                ? (body.DateTo.Value - body.DateFrom.Value).Days
                : 1;

            if (nights <= 0) nights = 1;

            foreach (var roomItem in body.RoomItems)
            {
                if (!roomItem.PricePerRoom.HasValue && roomItem.HotelRoomId.HasValue && roomItem.FeedingTypeId.HasValue)
                {
                    roomItem.PricePerRoom = CalculatePriceForRoomType(
                        roomItem.HotelRoomId.Value,
                        roomItem.FeedingTypeId.Value,
                        nights);
                }
            }

            body.TotalPrice = CalculateTotalPriceFromRoomItems(body.RoomItems);
        }

        public void ValidatePaymentItems(ReservationUM body)
        {
            if (body.PaymentItems == null || !body.PaymentItems.Any())
            {
                throw new ArgumentException("At least one payment must be specified");
            }

            var totalPayment = CalculateTotalPaymentFromItems(body.PaymentItems);
            var totalPrice = body.TotalPrice ?? 0;

            if (totalPayment > totalPrice)
            {
                throw new ArgumentException("Total payment amount cannot exceed total price");
            }
        }

        public void UpdateReservationTotalPrice(int reservationId)
        {
            var reservation = _dbContext.Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation != null)
            {
                reservation.TotalPrice = CalculateTotalPriceFromRooms(reservationId);
            }
        }
    }
}
