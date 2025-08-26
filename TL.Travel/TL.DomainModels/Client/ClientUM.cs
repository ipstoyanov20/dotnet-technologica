using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Client
{
    public class ClientUM
    {
        [StringLength((128))]
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength((128))]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [StringLength((15))]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string Phone { get; set; }
        public bool isActive { get; set; }
    }
}
