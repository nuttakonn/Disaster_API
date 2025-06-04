using Disaster_API.Interfaces;
using Disaster_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Disaster_API.Controllers
{
    [ApiController]
    [Route("api/trucks")]
    public class TrucksController : ControllerBase
    {
        private readonly IAssignmentService _service;
        public TrucksController(IAssignmentService service) => _service = service;

        [HttpPost]
        public IActionResult AddTruck([FromBody] List<Truck> truck)
        {
            var added = _service.AddTruck(truck);
            if (added)
                return Ok("Truck added.");
            else
                return BadRequest("TruckId already exists. Cannot add duplicate.");
        }
    }
}