using System.ComponentModel.DataAnnotations;
using TaskTrackerAPI.Domain.Enums;
using TaskStatus = TaskTrackerAPI.Domain.Enums.TaskStatus;

namespace TaskTrackerAPI.Domain.Dtos
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(TaskStatus), ErrorMessage = "Status must be one of: New, InProgress, Done")]
        public TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [EnumDataType(typeof(Priority), ErrorMessage = "Priority must be one of: Low, Medium, High")]
        public Priority Priority { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Due date must be a valid ISO-8601 date")]
        public DateTime? DueDate { get; set; }
    }
}
