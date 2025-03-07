namespace TaskManagementApp.Application.DTOs
{
    public class TaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
    }
}
