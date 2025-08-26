using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.Location;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class LocationController : BaseController
    {
        readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var locations = _locationService.GetAll();
            return Ok(locations);
        }

        [HttpGet]

        public IActionResult GetById([FromQuery] int id)
        {
            var location = _locationService.GetById(id);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] LocationUM location, int id = 0)
        {
            LocationVM result;
            if (id > 0)
            {
                result = _locationService.AddEdit(location, id);
            }
            else
            {
                result = _locationService.AddEdit(location);
            }


            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            _locationService.Delete(id);
            return NoContent();
        }
    }
}
