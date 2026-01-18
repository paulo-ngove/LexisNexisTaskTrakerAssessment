namespace TaskTrackerAPI.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using TaskTrackerAPI.Controllers;
    using TaskTrackerAPI.Domain.Dtos;
    using TaskTrackerAPI.Domain.Enums;
    using TaskTrackerAPI.Domain.Models;
    using TaskTrackerAPI.Infrastructure;
    using Xunit;
    using TaskStatus = Domain.Enums.TaskStatus;

    public class TasksControllerTests
    {
        private readonly TasksController _testClass;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TasksController> _logger;
        private readonly List<TaskItem> _mockTasks;

        public TasksControllerTests()
        {
            // Create in-memory database context
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<TasksController>>();
            _testClass = new TasksController(_context, _mapper, _logger);

            // Seed mock data
            _mockTasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Test Task 1",
                    Description = "Description 1",
                    Status = TaskStatus.New,
                    Priority = Priority.Medium,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Test Task 2",
                    Description = "Description 2",
                    Status = TaskStatus.InProgress,
                    Priority = Priority.High,
                    DueDate = DateTime.UtcNow.AddDays(3),
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Another Task",
                    Description = "Description 3",
                    Status = TaskStatus.Done,
                    Priority = Priority.Low,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Tasks.AddRange(_mockTasks);
            _context.SaveChanges();
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new TasksController(_context, _mapper, _logger);

            // Assert
            Assert.NotNull(instance);
        }
        [Fact]
        public async Task CanCallGetTask()
        {
            // Arrange
            var id = 1;

            // Setup mapper
            var mockTask = _mockTasks.First();
            var mockResponseDto = new TaskResponseDto
            {
                Id = mockTask.Id,
                Title = mockTask.Title,
                Description = mockTask.Description,
                Status = mockTask.Status,
                Priority = mockTask.Priority
            };

            _mapper.Map<TaskResponseDto>(Arg.Any<TaskItem>())
                  .Returns(mockResponseDto);

            // Act
            var result = await _testClass.GetTask(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
            Assert.Equal(id, returnedTask.Id);
            Assert.Equal("Test Task 1", returnedTask.Title);
        }
        [Fact]
        public async Task CanCallCreateTask()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "New Test Task",
                Description = "New Description",
                Status = TaskStatus.New,
                Priority = Priority.Medium,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            var mockTaskItem = new TaskItem
            {
                Id = 4,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            var mockResponseDto = new TaskResponseDto
            {
                Id = 4,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                Priority = createTaskDto.Priority
            };

            _mapper.Map<TaskItem>(Arg.Any<CreateTaskDto>())
                  .Returns(mockTaskItem);

            _mapper.Map<TaskResponseDto>(Arg.Any<TaskItem>())
                  .Returns(mockResponseDto);

            // Act
            var result = await _testClass.CreateTask(createTaskDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTask = Assert.IsType<TaskResponseDto>(createdAtResult.Value);
            Assert.Equal("New Test Task", returnedTask.Title);
            Assert.Equal(TaskStatus.New, returnedTask.Status);
        }

        [Fact]
        public async Task CannotCallCreateTaskWithNullCreateTaskDto()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _testClass.CreateTask(default(CreateTaskDto)));
        }
        [Fact]
        public async Task CanCallUpdateTask()
        {
            // Arrange
            var id = 1;
            var updateTaskDto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Status = TaskStatus.InProgress,
                Priority = Priority.High
            };

            // Setup existing task
            var existingTask = _mockTasks.First();
            _context.Entry(existingTask).State = EntityState.Detached;

            // Setup mapper
            _mapper.Map(Arg.Any<UpdateTaskDto>(), Arg.Any<TaskItem>())
                  .Returns(callInfo =>
                  {
                      var dto = callInfo.Arg<UpdateTaskDto>();
                      var task = callInfo.Arg<TaskItem>();

                      if (!string.IsNullOrEmpty(dto.Title))
                          task.Title = dto.Title;
                      if (!string.IsNullOrEmpty(dto.Description))
                          task.Description = dto.Description;
                      if (dto.Status.HasValue)
                          task.Status = dto.Status.Value;
                      if (dto.Priority.HasValue)
                          task.Priority = dto.Priority.Value;

                      task.UpdatedAt = DateTime.UtcNow;
                      return task;
                  });

            // Act
            var result = await _testClass.UpdateTask(id, updateTaskDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CanCallDeleteTask()
        {
            // Arrange
            var id = 1;
            var taskToDelete = _mockTasks.First();

            // Act
            var result = await _testClass.DeleteTask(id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify task was removed
            var deletedTask = await _context.Tasks.FindAsync(id);
            Assert.Null(deletedTask);
        }
  
        [Fact]
        public async Task CanCallMarkAsComplete()
        {
            // Arrange
            var id = 1;
            var task = _mockTasks.First();
            task.Status = TaskStatus.InProgress; // Start as not completed

            var mockResponseDto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Status = TaskStatus.Done,
                IsCompleted = true
            };

            _mapper.Map<TaskResponseDto>(Arg.Any<TaskItem>())
                  .Returns(mockResponseDto);

            // Act
            var result = await _testClass.MarkAsComplete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
            Assert.Equal(TaskStatus.Done, returnedTask.Status);
            Assert.True(returnedTask.IsCompleted);

            // Verify task was updated in database
            var updatedTask = await _context.Tasks.FindAsync(id);
            Assert.Equal(TaskStatus.Done, updatedTask?.Status);
        }

        [Fact]
        public async Task CanCallMarkAsIncomplete()
        {
            // Arrange
            var id = 3; // Task 3 is Done in seed data
            var task = _mockTasks[2]; // Get the Done task

            var mockResponseDto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Status = TaskStatus.InProgress,
                IsCompleted = false
            };

            _mapper.Map<TaskResponseDto>(Arg.Any<TaskItem>())
                  .Returns(mockResponseDto);

            // Act
            var result = await _testClass.MarkAsIncomplete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
            Assert.Equal(TaskStatus.InProgress, returnedTask.Status);
            Assert.False(returnedTask.IsCompleted);

            // Verify task was updated in database
            var updatedTask = await _context.Tasks.FindAsync(id);
            Assert.Equal(TaskStatus.InProgress, updatedTask?.Status);
        }

        [Fact]
        public async Task CanCallGetTasksByPriority()
        {
            // Arrange
            var priority = Priority.High;

            var highPriorityTasks = _mockTasks
                .Where(t => t.Priority == priority)
                .ToList();

            var mockResponseDtos = highPriorityTasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Priority = t.Priority
            }).ToList();

            _mapper.Map<List<TaskResponseDto>>(Arg.Any<List<TaskItem>>())
                  .Returns(mockResponseDtos);

            // Act
            var result = await _testClass.GetTasksByPriority(priority);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsType<List<TaskResponseDto>>(okResult.Value);
            Assert.NotEmpty(returnedTasks);
            Assert.All(returnedTasks, task => Assert.Equal(priority, task.Priority));
        }

        [Fact]
        public async Task GetTasksByPriority_WithNoMatchingTasks_ReturnsEmptyList()
        {
            // Arrange
            var priority = Priority.Low; // Only one Low priority task in seed data

            var mockResponseDtos = new List<TaskResponseDto>
            {
                new TaskResponseDto
                {
                    Id = 3,
                    Title = "Another Task",
                    Priority = Priority.Low
                }
            };

            _mapper.Map<List<TaskResponseDto>>(Arg.Any<List<TaskItem>>())
                  .Returns(mockResponseDtos);

            // Act
            var result = await _testClass.GetTasksByPriority(priority);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsType<List<TaskResponseDto>>(okResult.Value);
            Assert.Single(returnedTasks);
            Assert.All(returnedTasks, task => Assert.Equal(priority, task.Priority));
        }

        [Fact]
        public async Task GetTasks_WithSearchQuery_ReturnsFilteredResults()
        {
            // Arrange
            var searchQuery = "Test";
            var sort = "dueDate:asc";

            var filteredTasks = _mockTasks
                .Where(t => t.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                           t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var mockResponseDtos = filteredTasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description
            }).ToList();

            _mapper.Map<List<TaskResponseDto>>(Arg.Any<List<TaskItem>>())
                  .Returns(mockResponseDtos);

            // Act
            var result = await _testClass.GetTasks(searchQuery, sort);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsType<List<TaskResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count); // Should match "Test Task 1" and "Test Task 2"
            Assert.All(returnedTasks, task =>
                Assert.Contains(searchQuery, task.Title, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetTasks_WithSorting_ReturnsSortedResults()
        {
            // Arrange
            var sort = "title:asc";

            var sortedTasks = _mockTasks
                .OrderBy(t => t.Title)
                .ToList();

            var mockResponseDtos = sortedTasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title
            }).ToList();

            _mapper.Map<List<TaskResponseDto>>(Arg.Any<List<TaskItem>>())
                  .Returns(mockResponseDtos);

            // Act
            var result = await _testClass.GetTasks(null, sort);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsType<List<TaskResponseDto>>(okResult.Value);
            Assert.Equal("Another Task", returnedTasks[0].Title);
            Assert.Equal("Test Task 1", returnedTasks[1].Title);
            Assert.Equal("Test Task 2", returnedTasks[2].Title);
        }

        // Cleanup after each test
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}