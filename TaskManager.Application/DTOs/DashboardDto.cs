﻿

namespace TaskManager.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveUsers { get; set; }
    }
}
