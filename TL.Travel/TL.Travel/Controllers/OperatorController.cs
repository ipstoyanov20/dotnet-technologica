using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.Operators;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class OperatorController : BaseController
    {
        private IOperatorService operatorService;

        public OperatorController(IOperatorService operatorService)
        {
            this.operatorService = operatorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var res = operatorService.GetAll();
            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var res = operatorService.GetById(id);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] EditOperatorDTO entry, int id = 0)
        {
            if (id == 0)
            {
                var res = operatorService.AddOrUpdate(entry);
                return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
            }
            else
            {
                var res = operatorService.AddOrUpdate(entry, id);
                return Ok(res);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            operatorService.Delete(id);
            return NoContent();
        }
    }
}
