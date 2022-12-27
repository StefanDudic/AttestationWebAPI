namespace AttestationProject.Interfaces.Services
{
    public interface ITaskService
    {
        Task CreateTaskAsync(TaskInformationalModel taskToCreate, SkillName skillName);
    }
}
