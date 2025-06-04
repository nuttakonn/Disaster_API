using Disaster_API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Disaster_API.Controllers
{
    [ApiController]
    [Route("api/assignments")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _service;
        public AssignmentsController(IAssignmentService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> ProcessAssignments()
        {
            var result = await _service.ProcessAssignmentsAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var result = await _service.GetAssignmentsAsync();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAssignments()
        {
            await _service.ClearAssignmentsCacheAsync();
            return Ok("Cache cleared.");
        }
    }
}