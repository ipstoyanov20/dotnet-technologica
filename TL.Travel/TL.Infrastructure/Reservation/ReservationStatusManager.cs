using System;
using System.Linq;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;

namespace TL.Travel.Infrastructure.Reservations
{
    public class ReservationStatusManager
    {
        private readonly BaseTLTravelDbContext _dbContext;

        public ReservationStatusManager(BaseTLTravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int DetermineReservationStatus(ReservationUM body, decimal totalPrice)
        {
            if (body.ReservationStatusId.HasValue)
            {
                var requestedStatus = _dbContext.ReservationStatuses.FirstOrDefault(rs => rs.Id == body.ReservationStatusId.Value && rs.IsActive);
                if (requestedStatus == null)
                    throw new ArgumentException("Invalid reservation status");

                if (body.ReservationStatusId.Value == ReservationConstants.STATUS_CANCELED)
                    return ReservationConstants.STATUS_CANCELED;
            }

            var totalPaymentAmount = body.PaymentItems?.Sum(pi => pi.Amount ?? 0) ?? 0;

            if (totalPaymentAmount == 0 || totalPrice <= 0)
                return ReservationConstants.STATUS_UNPAID;

            var paymentRatio = totalPaymentAmount / totalPrice;

            if (paymentRatio == 0)
                return ReservationConstants.STATUS_UNPAID;
            else if (paymentRatio >= 1.0m)
                return ReservationConstants.STATUS_FULLPAID;
            else if (paymentRatio > 0)
                return ReservationConstants.STATUS_PARTPAY;
            else
                return ReservationConstants.STATUS_UNPAID;
        }

        public int DetermineReservationStatus(ReservationUM body)
        {
            return DetermineReservationStatus(body, body.TotalPrice ?? 0);
        }

        public int DetermineReservationStatusFromDatabase(int reservationId)
        {
            var reservation = _dbContext.Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                return ReservationConstants.STATUS_UNPAID;

            var totalPrice = reservation.TotalPrice;
            var totalPaidAmount = _dbContext.ReservationPayments
                .Where(rp => rp.ReservationId == reservationId && rp.IsActive && rp.IsPaid)
                .Sum(rp => rp.Amount);

            if (totalPaidAmount == 0 || totalPrice <= 0)
                return ReservationConstants.STATUS_UNPAID;

            var paymentRatio = totalPaidAmount / totalPrice;

            if (paymentRatio >= 1.0m)
                return ReservationConstants.STATUS_FULLPAID;
            else if (paymentRatio > 0)
                return ReservationConstants.STATUS_PARTPAY;
            else
                return ReservationConstants.STATUS_UNPAID;
        }
    }
}
