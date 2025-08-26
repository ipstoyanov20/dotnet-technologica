using Microsoft.AspNetCore.Mvc;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Feeding;
using TL.Travel.DomainModels.Payment;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class PaymentTypeController : BaseController
    {
        readonly IPaymentTypeService service;

        public PaymentTypeController(IPaymentTypeService service)
        {
            this.service = service;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var paymentTypes = service.GetAll();
            return Ok(paymentTypes);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var paymentType = service.GetById(id);
            if (paymentType == null)
                return NotFound();
            return Ok(paymentType);


        }


        [HttpPost]
        public IActionResult AddEdit([FromBody] PaymentUM paymentType, int id = 0)
        {
            PaymentVM result;
            if (id > 0)
                result = service.AddEdit(paymentType, id);
            else
                result = service.AddEdit(paymentType);


            return Ok(result);
        }
        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            var deleted = service.Delete(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }


    }
}
