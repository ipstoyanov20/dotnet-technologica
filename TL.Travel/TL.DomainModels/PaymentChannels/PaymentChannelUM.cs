using System.ComponentModel.DataAnnotations;

namespace TL.Travel.DomainModels.PaymentChannels
{
    public class PaymentChannelUM
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
