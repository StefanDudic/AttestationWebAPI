namespace AttestationProject.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<AsyncPageable<SkillRecord>> GetTaskByIdAsync(string id);

    }
}
