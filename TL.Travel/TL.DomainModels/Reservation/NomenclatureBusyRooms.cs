using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.Reservation
{
    public class NomenclatureBusyRooms
    {

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int? RoomId { get; set; }

        public int? RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }

        public int? HotelId { get; set; }

        public string? HotelName { get; set; }

    }
}
