using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.Extras;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class ExtrasController : BaseController
    {
        private readonly IExtrasService _extrasService;

        public ExtrasController(IExtrasService extrasService)
        {
            _extrasService = extrasService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var extras = _extrasService.GetAllExtras();
            return Ok(extras);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            try
            {
                var extra = _extrasService.GetExtraById(id);
                return Ok(extra);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] ExtrasUM extra, int id = 0)
        {
            try
            {
                var result = _extrasService.AddEdit(extra, id);
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
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                await _extrasService.DeleteExtra(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
