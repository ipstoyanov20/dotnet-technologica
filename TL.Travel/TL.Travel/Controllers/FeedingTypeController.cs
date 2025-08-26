using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.Feeding;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class FeedingTypeController : BaseController
    {
        readonly IFeedingTypeService service;
        public FeedingTypeController(IFeedingTypeService service)
        {
            this.service = service;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var feedingTypes = service.GetAll();
            return Ok(feedingTypes);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var feedingType = service.GetById(id);
            if (feedingType == null)
                return NotFound();
            return Ok(feedingType);
        }


        [HttpPost]
        public IActionResult AddEdit([FromBody] FeedingUM feedingType, int id = 0)
        {
            FeedingVM result;
            if (id > 0)
                result = service.AddEdit(feedingType, id);
            else
                result = service.AddEdit(feedingType);


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
