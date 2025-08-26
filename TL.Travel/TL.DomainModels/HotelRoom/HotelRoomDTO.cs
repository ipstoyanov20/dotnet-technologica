using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.HotelRoom
{
    public class HotelRoomDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

    }
}
