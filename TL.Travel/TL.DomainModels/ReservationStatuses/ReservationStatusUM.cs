using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.ReservationStatuses
{
    public class ReservationStatusUM
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
