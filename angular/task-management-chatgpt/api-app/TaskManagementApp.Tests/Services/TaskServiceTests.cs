using Moq;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.Services;
using TaskManagementApp.Core.Entities;
using Xunit;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateTask_ShouldReturnTaskDto_WhenValidInput()
    {
        var taskRepoMock = new Mock<ITaskRepository>();
        var taskService = new TaskService(taskRepoMock.Object);
        
        var taskDto = new TaskDto
        {
            Title = "New Task",
            Description = "Description of the task",
            IsCompleted = false,
            DueDate = DateTime.Now.AddDays(1),
            Priority = "Medium"
        };
        
        var task = new Task
        {
            Id = 1,
            Title = taskDto.Title,
            Description = taskDto.Description,
            IsCompleted = taskDto.IsCompleted,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority
        };
        
        taskRepoMock.Setup(repo => repo.AddTask(It.IsAny<Task>())).ReturnsAsync(task);
        
        var result = await taskService.CreateTask(taskDto);
        
        Assert.Equal(taskDto.Title, result.Title);
    }
}
