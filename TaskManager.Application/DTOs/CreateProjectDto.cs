﻿

namespace TaskManager.Application.DTOs
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}
