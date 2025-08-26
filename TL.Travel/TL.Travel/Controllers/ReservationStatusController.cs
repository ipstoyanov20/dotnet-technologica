using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.ReservationStatuses;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class ReservationStatusController : BaseController
    {
        private readonly IReservationStatusService _reservationStatusService;

        public ReservationStatusController(IReservationStatusService reservationStatusService)
        {
            _reservationStatusService = reservationStatusService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var reservationStatuses = _reservationStatusService.GetAll();
            return Ok(reservationStatuses);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            try
            {
                var reservationStatus = _reservationStatusService.GetById(id);
                return Ok(reservationStatus);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] ReservationStatusUM reservationStatus, int id = 0)
        {
            try
            {
                var result = _reservationStatusService.AddEdit(reservationStatus, id);
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
                var result = _reservationStatusService.Delete(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
