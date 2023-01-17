namespace AttestationProject.Interfaces.Services
{
    public interface ITaskService
    {
        Task CreateTaskAsync(TaskInformationalModel taskToCreate, SkillName skillName);
        Task<TaskInformationalModel> GetTaskByIdAsync(Guid skillId, Guid taskId);
        Task<List<TaskInformationalModel>> GetAllTasksAsync();
        Task UpdateTaskAsync(TaskModel taskModel, Guid skillId);
    }
}
