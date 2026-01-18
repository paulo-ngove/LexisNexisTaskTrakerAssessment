using TaskTrackerAPI.Domain.Enums;
using TaskStatus = TaskTrackerAPI.Domain.Enums.TaskStatus;

namespace TaskTrackerAPI.Domain.Dtos
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
