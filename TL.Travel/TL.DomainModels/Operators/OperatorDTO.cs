using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Operators
{
    public class OperatorDTO
    {
        public decimal? Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }


        [Required]
        public decimal? ComissionPercent { get; set; }

        public bool IsActive { get; set; }
    }
}
