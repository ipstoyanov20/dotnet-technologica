using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TL.Travel.DomainModels.Client;
using TL.Travel.Interfaces;

namespace TL.TravelAPI.Controllers
{
    public class ClientController : BaseController
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var clients = _clientService.GetAll();
            return Ok(clients);
        }

        [HttpGet]
        public IActionResult GetById([FromQuery] int id)
        {
            var client = _clientService.GetById(id);

            return Ok(client);
        }

        [HttpPost]
        public IActionResult AddEdit([FromBody] ClientUM client, int id = 0)
        {

            var result = _clientService.AddEdit(client, id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int id)
        {
            _clientService.Delete(id);
            return NoContent();

        }






    }
}
