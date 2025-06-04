using Disaster_API.Interfaces;
using Disaster_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Disaster_API.Controllers
{
    [ApiController]
    [Route("api/areas")]
    public class AreasController : ControllerBase
    {
        private readonly IAssignmentService _service;
        public AreasController(IAssignmentService service) => _service = service;

        [HttpPost]
        public IActionResult AddArea([FromBody] List<Area> area)
        {
            var added = _service.AddArea(area);
            if (added)
                return Ok("Area added.");
            else
                return BadRequest("AreaId already exists. Cannot add duplicate.");
        }

    }
}