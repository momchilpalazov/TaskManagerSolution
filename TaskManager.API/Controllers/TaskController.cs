using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Utils;

namespace TaskManager.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            var result = await _taskService.CreateTaskAsync(dto);
            return Ok(result);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByProject(Guid projectId)
        {
            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
            return Ok(tasks);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> FilterTasks([FromQuery] FilterTasksDto filter)
        {
            var tasks = await _taskService.FilterTasksAsync(filter);
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
        {
            var updated = await _taskService.UpdateTaskAsync(id, dto);
            if (!updated) return NotFound(new { message = "Task not found." });
            return NoContent(); // 204
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted) return NotFound(new { message = "Task not found." });
            return NoContent(); // 204
        }

        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetMyTasks()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var tasks = await _taskService.GetTasksForUserAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetOverdueTasks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var tasks = await _taskService.GetOverdueTasksAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportTasksAsCsv()
        {
            var tasks = await _taskService.GetAllTasksAsync(); // или само на текущия потребител
            var csv = CsvExporter.ExportTasksToCsv(tasks);

            return File(csv, "text/csv", "tasks_export.csv");
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> SearchTasks([FromQuery] string keyword)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var results = await _taskService.SearchTasksAsync(userId, keyword);
            return Ok(results);
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTodayTasks()
        {
            var userId = GetUserIdFromToken();
            var tasks = await _taskService.GetTasksForTodayAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("tomorrow")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTomorrowTasks()
        {
            var userId = GetUserIdFromToken();
            var tasks = await _taskService.GetTasksForTomorrowAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("week")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetWeekTasks()
        {
            var userId = GetUserIdFromToken();
            var tasks = await _taskService.GetTasksForWeekAsync(userId);
            return Ok(tasks);
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim!.Value);
        }

        [HttpGet("by-label/{labelId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetByLabel(Guid labelId)
        {
            var userId = GetUserIdFromToken();
            var tasks = await _taskService.GetTasksByLabelAsync(userId, labelId);
            return Ok(tasks);
        }




    }
}
