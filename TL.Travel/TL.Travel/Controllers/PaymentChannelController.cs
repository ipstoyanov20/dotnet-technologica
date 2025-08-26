using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.PaymentChannels;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class PaymentChannelController : BaseController
    {
        private readonly IPaymentChannelService _paymentChannelService;

        public PaymentChannelController(IPaymentChannelService paymentChannelService)
        {
            _paymentChannelService = paymentChannelService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var paymentChannels = _paymentChannelService.GetAll();
            return Ok(paymentChannels);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            try
            {
                var paymentChannel = _paymentChannelService.GetById(id);
                return Ok(paymentChannel);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] PaymentChannelUM paymentChannel, int id = 0)
        {
            try
            {
                var result = _paymentChannelService.AddEdit(paymentChannel, id);
                if (id == 0)
                {
                    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            try
            {
                var result = _paymentChannelService.Delete(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
