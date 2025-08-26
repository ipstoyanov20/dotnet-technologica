using System;

namespace TL.Travel.DomainModels.Reservation
{
    public class ReservationVM
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string CustomerNotes { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public int ReservationStatusId { get; set; }
        public string ReservationStatusName { get; set; }
    }
}
