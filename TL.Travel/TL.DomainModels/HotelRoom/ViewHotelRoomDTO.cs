using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.HotelRoom
{
    public class ViewHotelRoomDTO
    {
        public int Id { get; set; }


        public int HotelId { get; set; }


        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int? MaxAdults { get; set; }

        public int? MaxChildren { get; set; }

        public int? MaxBabies { get; set; }
    }
}
