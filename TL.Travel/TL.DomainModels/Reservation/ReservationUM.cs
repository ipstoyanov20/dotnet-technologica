using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TL.Travel.DomainModels.Reservation
{
    public class ReservationUM
    {
        [Required]
        public DateTime? DateFrom { get; set; }

        [Required]
        public DateTime? DateTo { get; set; }

        public string CustomerNotes { get; set; }

        public decimal? TotalPrice { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int? ClientId { get; set; }

        [Required]
        public int? OperatorId { get; set; }

        public int? ReservationStatusId { get; set; }

        [Required]
        public int? HotelId { get; set; }

        [Required]
        public List<ReservationRoomItemUM> RoomItems { get; set; } = new List<ReservationRoomItemUM>();

        public int? Adults => RoomItems?.Sum(ri => (ri.Adults ?? 0) * (ri.RoomCount ?? 0));
        public int? Children => RoomItems?.Sum(ri => (ri.Children ?? 0) * (ri.RoomCount ?? 0));
        public int? Babies => RoomItems?.Sum(ri => (ri.Babies ?? 0) * (ri.RoomCount ?? 0));


        [Required]
        public List<ReservationPaymentItemUM> PaymentItems { get; set; } = new List<ReservationPaymentItemUM>();

        public decimal? PaymentAmount => PaymentItems?.Sum(pi => pi.Amount ?? 0);


    }
}
