

using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateTaskAsync_ShouldReturnTaskDto_WhenValidInput()
        {
            // Arrange
            var fakeUnitOfWork = new Mock<IUnitOfWork>();
            var fakeEmailService = new Mock<IEmailService>();
            var fakeAuditService = new Mock<IAuditService>();

            var assignedUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@user.com"
            };

            fakeUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(assignedUser);

            fakeUnitOfWork.Setup(u => u.TaskItems.AddAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);

            fakeUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            var service = new TaskService(
                fakeUnitOfWork.Object,
                fakeEmailService.Object,
                fakeAuditService.Object
            );

            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Testing",
                ProjectId = Guid.NewGuid(),
                AssignedToUserId = assignedUser.Id,
                Status = "Todo",
                Priority = "High",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            // Act
            var result = await service.CreateTaskAsync(dto);

            // Assert
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Status, result.Status);
            Assert.Equal(dto.Priority, result.Priority);
            Assert.Equal(assignedUser.Email, result.AssignedToEmail);
        }
    }
}
