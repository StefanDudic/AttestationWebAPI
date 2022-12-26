namespace AttestationProject.Interfaces.Repositories
{
    public interface ISkillRepository
    {
        Task CreateSkillAsync(SkillRecord skillToCreate);
        Task<AsyncPageable<SkillRecord>> GetSkillByIdAsync(string id);
        Task<AsyncPageable<SkillRecord>> GetSkillByNameAsync(string skillName);
        Task<AsyncPageable<SkillRecord>> GetAllSkillsAsync();
        Task UpdateSkillAsync(SkillRecord skillRecordToUpdate);
        Task DeleteSkillAsync(List<SkillRecord> skillRecordResponse);
    }
}
