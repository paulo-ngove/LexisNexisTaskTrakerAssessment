using System.ComponentModel.DataAnnotations;
using TaskTrackerAPI.Domain.Enums;
using TaskStatus = TaskTrackerAPI.Domain.Enums.TaskStatus;

namespace TaskTrackerAPI.Domain.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.New;

        [Required]
        public Priority Priority { get; set; } = Priority.Medium;

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsCompleted => Status == TaskStatus.Done;
    }
}
