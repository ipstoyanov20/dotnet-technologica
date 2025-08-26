using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.Hotel
{
    public class HotelVM
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string DestinationName { get; set; }

        public int Stars { get; set; }

        public string Contacts { get; set; }

        public bool IsTemporaryClosed { get; set; }
    }
}
