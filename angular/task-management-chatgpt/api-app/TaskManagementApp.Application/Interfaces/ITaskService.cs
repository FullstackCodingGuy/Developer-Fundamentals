namespace TaskManagementApp.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTask(TaskDto taskDto);
        Task<TaskDto> UpdateTask(int taskId, TaskDto taskDto);
        Task<bool> DeleteTask(int taskId);
        Task<TaskDto> GetTaskById(int taskId);
        Task<IEnumerable<TaskDto>> GetAllTasks();
    }
}
