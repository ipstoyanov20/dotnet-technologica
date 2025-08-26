using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Reservation
{
    public class ReservationRoomItemUM
    {
        [Required]
        public int? HotelRoomId { get; set; }
        
        [Required]
        public int? FeedingTypeId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Room count must be at least 1")]
        public int? RoomCount { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "At least one adult is required")]
        public int? Adults { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Children count cannot be negative")]
        public int? Children { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Babies count cannot be negative")]
        public int? Babies { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per room must be greater than zero")]
        public decimal? PricePerRoom { get; set; }
    }
}
