using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.DTOs;
using TaskManagementApp.Core.Interfaces;
using TaskManagementApp.Core.Entities;

namespace TaskManagementApp.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskDto> CreateTask(TaskDto taskDto)
        {
            var task = new Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority
            };

            await _taskRepository.AddTask(task);
            return taskDto;
        }

        public async Task<TaskDto> UpdateTask(int taskId, TaskDto taskDto)
        {
            var existingTask = await _taskRepository.GetTaskById(taskId);
            if (existingTask == null)
                throw new KeyNotFoundException("Task not found");

            existingTask.Title = taskDto.Title;
            existingTask.Description = taskDto.Description;
            existingTask.IsCompleted = taskDto.IsCompleted;
            existingTask.DueDate = taskDto.DueDate;
            existingTask.Priority = taskDto.Priority;

            await _taskRepository.UpdateTask(existingTask);
            return taskDto;
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            var task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            await _taskRepository.DeleteTask(taskId);
            return true;
        }

        public async Task<TaskDto> GetTaskById(int taskId)
        {
            var task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            return new TaskDto
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                Priority = task.Priority
            };
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllTasks();
            return tasks.Select(task => new TaskDto
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                Priority = task.Priority
            });
        }
    }
}
