using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Reservation
{
    public class ReservationPaymentItemUM
    {
        [Required]
        public int? PaymentTypeId { get; set; }
        
        [Required]
        public int? PaymentChannelId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero")]
        public decimal? Amount { get; set; }
        
        [Required]
        public DateTime? DueDate { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
    }
}
