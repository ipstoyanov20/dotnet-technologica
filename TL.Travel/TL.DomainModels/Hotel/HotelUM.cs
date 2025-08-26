using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TL.Travel.DomainModels.Hotel
{
    public class HotelUM
    {
        //imash na reservaton da pokavesh idto i tipa na vs koito sa zaedti spraqmo datefrom and dateto

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; }

        [Required]
        [StringLength(500)]
        public string Contacts { get; set; }

        public bool IsTemporaryClosed { get; set; }

        public List<IFormFile> Photo { get; set; }
        public List<int> Extras { get; set; }
        public List<int> FeedingsTypeId { get; set; }

        public int? PartnerId { get; set; }
        public int? LocationId { get; set; }





    }
}
