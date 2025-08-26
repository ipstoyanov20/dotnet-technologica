using Microsoft.AspNetCore.Mvc;
using TL.Travel.DomainModels.Reservation;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class ReservationController : BaseController
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var reservations = _reservationService.GetAll();
            return Ok(reservations);
        }

        [HttpPost]
        public IActionResult GetAllBusyRooms([FromBody] NomenclatureBusyRooms body)
        {
            var reservations = _reservationService.GetAllBusyRooms(body);
            return Ok(reservations);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            try
            {
                var reservation = _reservationService.GetById(id);
                return Ok(reservation);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] ReservationUM reservation, [FromQuery] int id = 0)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => new
                    {
                        Field = kvp.Key,
                        Errors = kvp.Value.Errors.Select(e => e.ErrorMessage)
                    }).ToList();

                return UnprocessableEntity(new { message = "Validation failed", errors });
            }

            try
            {
                var result = _reservationService.AddEdit(reservation, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            try
            {
                _reservationService.Delete(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
