using AttestationProject.Models;

namespace AttestationProject.Interfaces.Services
{
    public interface ISkillService
    {
        Task CreateSkillAsync(SkillInformationalModel skillToCreate);
        Task<SkillInformationalModel> GetSkillByIdAsync(Guid id);
        Task<SkillInformationalModel> GetSkillByNameAsync(SkillName skillName);
        Task<List<SkillInformationalModel>> GetAllSkillsAsync();
        Task UpdateSkillAsync(SkillUpdateModel skill);
        Task DeleteSkillAsync(string param);
    }
}
