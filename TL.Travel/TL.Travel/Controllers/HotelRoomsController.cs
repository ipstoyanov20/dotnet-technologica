using Microsoft.AspNetCore.Mvc;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Feeding;
using TL.Travel.DomainModels.HotelRoom;
using TL.Travel.Interfaces;
using TL.WebHelpers.Models.RequestPaging;

namespace TL.TravelAPI.Controllers
{

    public class HotelRoomsController : BaseController
    {
        private IHotelRoomService service;
        public HotelRoomsController(IHotelRoomService service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult GetAll([FromBody] GridRequestModel<HotelRoomFilters> request)
        {
            return PageResult(service.GetAll(request.Filters), request);
        }


        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var room = service.GetById(id);
            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpGet]
        public IActionResult DownloadHotelRoomPhotosPdf([FromQuery] int id)
        {
            try
            {
                var pdfBytes = service.GenerateHotelRoomPhotosPdf(id);
                return File(pdfBytes, "application/pdf", $"hotel_room_{id}_photos.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        public IActionResult AddEdit([FromForm] UpdateHotelRoomDTO room, int id = 0)
        {
            ViewHotelRoomDTO createdRoom;
            if (id > 0)
                createdRoom = service.AddEdit(room, id);
            else
                createdRoom = service.AddEdit(room);


            return Ok(createdRoom);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            var deleted = service.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet]
        public IActionResult GetHotelRoomImagesList([FromQuery] int id)
        {
            try
            {
                var imageInfos = service.GetHotelRoomImagesList(id);
                return Ok(imageInfos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult DownloadHotelRoomImage([FromQuery] int imageId)
        {
            try
            {
                var imageData = service.GetRoomPhoto(imageId, out string contentType, out string fileName);
                return File(imageData, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
