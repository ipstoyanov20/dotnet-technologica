using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.Operators
{
    public class EditOperatorDTO : OperatorDTO
    {
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}
