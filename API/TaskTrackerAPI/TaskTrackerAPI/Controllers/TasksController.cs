using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Domain.Dtos;
using TaskTrackerAPI.Domain.Enums;
using TaskTrackerAPI.Domain.Models;
using TaskTrackerAPI.Infrastructure;
using TaskStatus = TaskTrackerAPI.Domain.Enums.TaskStatus;
using System.Linq.Dynamic.Core;

namespace TaskTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            AppDbContext context,
            IMapper mapper,
            ILogger<TasksController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks with optional filtering and sorting
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskResponseDto>))]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasks(
            [FromQuery] string? q = null,
            [FromQuery] string? sort = "dueDate:asc")
        {
            try
            {
                var query = _context.Tasks.AsQueryable();

                // Search query (case-insensitive)
                if (!string.IsNullOrWhiteSpace(q))
                {
                    query = query.Where(t =>
                        EF.Functions.Like(t.Title, $"%{q}%") ||
                        EF.Functions.Like(t.Description, $"%{q}%"));
                }

                var sortOrder = sort ?? "dueDate:asc";
                var sortParts = sortOrder.Split(':');
                var sortField = sortParts[0];
                var sortDirection = sortParts.Length > 1 ? sortParts[1] : "asc";

                var validSortFields = new[] { "dueDate", "createdAt", "priority", "title" };
                if (!validSortFields.Contains(sortField))
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid sort field",
                        Detail = $"Sort field must be one of: {string.Join(", ", validSortFields)}",
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                    });
                }

                query = sortDirection == "desc"
                    ? query.OrderBy($"{sortField} desc")
                    : query.OrderBy($"{sortField}");

                var tasks = await query.ToListAsync();
                var response = _mapper.Map<List<TaskResponseDto>>(tasks);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                throw;
            }
        }

        /// <summary>
        /// Get a specific task by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                var response = _mapper.Map<TaskResponseDto>(task);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task with ID {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TaskResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto createTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(er => er.ErrorMessage)
                        });

                    return BadRequest(CreateProblemDetails(
                        StatusCodes.Status400BadRequest,
                        "Validation error",
                        "One or more validation errors occurred",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        new Dictionary<string, object> { ["errors"] = errors }));
                }

                var task = _mapper.Map<TaskItem>(createTaskDto);

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<TaskResponseDto>(task);

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(er => er.ErrorMessage)
                        });

                    return BadRequest(CreateProblemDetails(
                        StatusCodes.Status400BadRequest,
                        "Validation error",
                        "One or more validation errors occurred",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        new Dictionary<string, object> { ["errors"] = errors }));
                }

                var existingTask = await _context.Tasks.FindAsync(id);
                if (existingTask == null)
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                _mapper.Map(updateTaskDto, existingTask);
                existingTask.UpdatedAt = DateTime.UtcNow;

                _context.Entry(existingTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating task with ID {TaskId}", id);

                if (!await TaskExists(id))
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// Mark a task as complete
        /// </summary>
        [HttpPatch("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<TaskResponseDto>> MarkAsComplete(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                task.Status = TaskStatus.Done;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<TaskResponseDto>(task);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking task as complete with ID {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// Mark a task as incomplete
        /// </summary>
        [HttpPatch("{id}/incomplete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<TaskResponseDto>> MarkAsIncomplete(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Task not found",
                        $"Task with ID {id} was not found",
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"));
                }

                task.Status = TaskStatus.InProgress;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<TaskResponseDto>(task);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking task as incomplete with ID {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get tasks by priority
        /// </summary>
        [HttpGet("priority/{priority}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByPriority(Priority priority)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.Priority == priority)
                    .OrderBy(t => t.DueDate)
                    .ToListAsync();

                var response = _mapper.Map<List<TaskResponseDto>>(tasks);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks by priority {Priority}", priority);
                throw;
            }
        }

        private async Task<bool> TaskExists(int id)
        {
            return await _context.Tasks.AnyAsync(e => e.Id == id);
        }

        private ProblemDetails CreateProblemDetails(
            int status,
            string title,
            string detail,
            string type,
            Dictionary<string, object> extensions = null)
        {
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Type = type,
                Instance = HttpContext.Request.Path
            };

            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    problem.Extensions.Add(extension.Key, extension.Value);
                }
            }

            return problem;
        }
    }
}