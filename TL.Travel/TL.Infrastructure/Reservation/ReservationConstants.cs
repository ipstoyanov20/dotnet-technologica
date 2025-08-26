namespace TL.Travel.Infrastructure.Reservations
{
    public static class ReservationConstants
    {
        public const int STATUS_NEW = 1;
        public const int STATUS_UNPAID = 2;
        public const int STATUS_PARTPAY = 3;
        public const int STATUS_FULLPAID = 4;
        public const int STATUS_CANCELED = 5;

        public const int MAX_GUESTS_PER_RESERVATION = 10;
    }
}
