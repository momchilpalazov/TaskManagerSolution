using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/task/{taskId}/attachments")]
    public class TaskAttachmentController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public TaskAttachmentController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файлът е празен");

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var savePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            await using var stream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var attachment = new TaskAttachment
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{fileName}",
                TaskItemId = taskId
            };

            _context.TaskAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(new { attachment.Id, attachment.FileName, Url = attachment.FilePath });
        }

        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> Download(Guid taskId, Guid attachmentId)
        {
            var attachment = await _context.TaskAttachments.FindAsync(attachmentId);
            if (attachment == null || attachment.TaskItemId != taskId)
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, attachment.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound("Файлът не е намерен");

            var mime = "application/octet-stream";
            return PhysicalFile(filePath, mime, attachment.FileName);
        }
    }
}
