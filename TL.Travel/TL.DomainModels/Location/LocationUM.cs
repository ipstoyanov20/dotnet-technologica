using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.Location
{
    public class LocationUM
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public bool IsAbroad { get; set; }

        public bool IsActive { get; set; }
    }
}
