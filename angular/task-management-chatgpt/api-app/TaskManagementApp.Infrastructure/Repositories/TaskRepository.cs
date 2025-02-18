using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Core.Interfaces;
using TaskManagementApp.Core.Entities;
using TaskManagementApp.Infrastructure.Data;

namespace TaskManagementApp.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Task> AddTask(Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateTask(Task task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Task> GetTaskById(int taskId)
        {
            return await _context.Tasks.FindAsync(taskId);
        }

        public async Task<IEnumerable<Task>> GetAllTasks()
        {
            return await _context.Tasks.ToListAsync();
        }
    }
}
