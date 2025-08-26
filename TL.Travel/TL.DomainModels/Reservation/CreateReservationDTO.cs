using System;
using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Reservation
{
    public class CreateReservationDTO
    {
        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        public string CustomerNotes { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int OperatorId { get; set; }

        [Required]
        public int ReservationStatusId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
