namespace TaskManagementApp.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task AddTask(Task task);
        Task UpdateTask(Task task);
        Task DeleteTask(int taskId);
        Task<Task> GetTaskById(int taskId);
        Task<IEnumerable<Task>> GetAllTasks();
    }
}
