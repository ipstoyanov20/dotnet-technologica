using Microsoft.AspNetCore.Mvc;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Hotel;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class HotelController : BaseController
    {
        private IHotelService _hotelService;
        public HotelController(IHotelService _hotelService)
        {
            this._hotelService = _hotelService;
        }


        [HttpGet]
        public IActionResult GetAllOperators()
        {
            var operators = _hotelService.GetAllOperators();
            return Ok(operators);
        }

        [HttpGet]
        public IActionResult GetAllDestinations()
        {
            var destinations = _hotelService.GetAllDestinations();
            return Ok(destinations);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var hotel = _hotelService.GetById(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var hotel = _hotelService.GetAll();
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }




        [HttpPost]
        public IActionResult AddEdit([FromForm] HotelUM body, int id = 0)
        {
            HotelVM result;
            if (id > 0)
            {
                result = _hotelService.AddEdit(body, id);

            }
            else
            {
                result = _hotelService.AddEdit(body);
            }
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            var deleted = _hotelService.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetHotelImagesList([FromQuery] int id)
        {
            try
            {
                var imageInfos = _hotelService.GetHotelImagesList(id);
                return Ok(imageInfos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult DownloadHotelImage([FromQuery] int imageId)
        {
            try
            {
                var imageData = _hotelService.GetHotelPhoto(imageId, out string contentType, out string fileName);
                return File(imageData, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        public IActionResult DownloadHotelPhotosPdf([FromQuery] int id)
        {
            try
            {
                var pdfBytes = _hotelService.GenerateHotelPhotosPdf(id);
                return File(pdfBytes, "application/pdf", $"hotel_{id}_photos.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
