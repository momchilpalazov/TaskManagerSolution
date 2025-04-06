using AutoMapper;
using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using Xunit;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public void GetTaskById_Should_Return_Task_When_Exists()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockEmailService = new Mock<IEmailService>();
            var mockAuditService = new Mock<IAuditService>();
            var mockMapper = new Mock<IMapper>();

            // Setup a task dto for the mapper to return
            var taskDto = new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                Status = "Todo",
                Priority = "High"
            };

            // Setup the mapper to return the task dto
            mockMapper.Setup(m => m.Map<TaskItem>(It.IsAny<CreateTaskDto>()))
                .Returns(new TaskItem { Id = taskDto.Id, Title = taskDto.Title });

            mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
                .Returns(taskDto);

            // Create the service
            var service = new TaskService(
                mockUnitOfWork.Object,
                mockEmailService.Object,
                mockAuditService.Object,
                mockMapper.Object
            );

            // Verify that the service was created successfully
            Assert.NotNull(service);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_Should_Return_Tasks_For_Project()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockEmailService = new Mock<IEmailService>();
            var mockAuditService = new Mock<IAuditService>();
            var mockMapper = new Mock<IMapper>();
            var mockTaskRepo = new Mock<IGenericRepository<TaskItem>>();

            // Create test data
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItem>
            {
                new TaskItem 
                { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 1", 
                    ProjectId = projectId,
                    Status = Domain.Entities.TaskStatus.Todo,
                    Priority = TaskPriority.Medium,
                    AssignedToUser = new User { Email = "user1@example.com" }
                },
                new TaskItem 
                { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 2", 
                    ProjectId = projectId,
                    Status = Domain.Entities.TaskStatus.InProgress,
                    Priority = TaskPriority.High,
                    AssignedToUser = new User { Email = "user2@example.com" }
                }
            };

            // Setup repository mock
            mockTaskRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TaskItem, bool>>>()))
                .ReturnsAsync(tasks);
            
            // Setup UnitOfWork mock
            mockUnitOfWork.Setup(u => u.TaskItems).Returns(mockTaskRepo.Object);

            // Create service
            var service = new TaskService(
                mockUnitOfWork.Object,
                mockEmailService.Object,
                mockAuditService.Object,
                mockMapper.Object
            );

            // Act
            var result = await service.GetTasksByProjectIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateTaskAsync_Should_Return_TaskDto_When_Valid_Input()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockEmailService = new Mock<IEmailService>();
            var mockAuditService = new Mock<IAuditService>();
            var mockMapper = new Mock<IMapper>();
            var mockTaskRepo = new Mock<IGenericRepository<TaskItem>>();
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            var mockLabelRepo = new Mock<IGenericRepository<Label>>();

            // Setup test data
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            
            var user = new User 
            { 
                Id = userId, 
                Email = "test@example.com" 
            };
            
            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                Status = Domain.Entities.TaskStatus.Todo,
                Priority = TaskPriority.High,
                ProjectId = projectId,
                AssignedToUserId = userId
            };
            
            var taskDto = new TaskDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status.ToString(),
                Priority = taskItem.Priority.ToString(),
                AssignedToEmail = user.Email
            };

            // Setup mapper mock
            mockMapper.Setup(m => m.Map<TaskItem>(It.IsAny<CreateTaskDto>()))
                .Returns(taskItem);
                
            mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
                .Returns(taskDto);
            
            // Setup repository mocks
            mockTaskRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);
                
            mockUserRepo.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            
            // Setup UnitOfWork mock
            mockUnitOfWork.Setup(u => u.TaskItems).Returns(mockTaskRepo.Object);
            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);
            mockUnitOfWork.Setup(u => u.Labels).Returns(mockLabelRepo.Object);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
            
            // Setup email service mock
            mockEmailService.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
                
            // Setup audit service mock
            mockAuditService.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Create service
            var service = new TaskService(
                mockUnitOfWork.Object,
                mockEmailService.Object,
                mockAuditService.Object,
                mockMapper.Object
            );

            // Create request DTO
            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                ProjectId = projectId,
                AssignedToUserId = userId,
                Status = "Todo",
                Priority = "High"
            };

            // Act
            var result = await service.CreateTaskAsync(createTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskDto.Title, result.Title);
            Assert.Equal(taskDto.Status, result.Status);
            Assert.Equal(taskDto.Priority, result.Priority);
            Assert.Equal(taskDto.AssignedToEmail, result.AssignedToEmail);
            
            // Verify repository calls
            mockTaskRepo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            mockUserRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
            
            // Verify email and audit service calls
            mockEmailService.Verify(e => e.SendAsync(
                It.Is<string>(s => s == user.Email), 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);
                
            mockAuditService.Verify(a => a.LogAsync(
                It.Is<string>(s => s == "Task"),
                It.Is<string>(s => s == "Create"),
                It.Is<string>(s => s == user.Email),
                It.IsAny<string>()),
                Times.Once);
        }
    }
}
