using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _labelService;

        public LabelController(ILabelService labelService)
        {
            _labelService = labelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabelDto>>> GetAll()
        {
            var labels = await _labelService.GetAllAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<ActionResult<LabelDto>> Create([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name cannot be empty.");

            var label = await _labelService.CreateAsync(name);
            return CreatedAtAction(nameof(GetAll), new { id = label.Id }, label);
        }
    }
}
