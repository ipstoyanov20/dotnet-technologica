using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TL.Travel.DomainModels.HotelRoom
{
    public class UpdateHotelRoomDTO
    {

        public int HotelId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int? MaxAdults { get; set; }

        public int? MaxChildren { get; set; }

        public int? MaxBabies { get; set; }
        public List<int> RoomExtras { get; set; }
        public List<IFormFile> RoomPhoto { get; set; }




    }
}
