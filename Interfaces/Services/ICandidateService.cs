namespace AttestationProject.Interfaces.Services
{
    public interface ICandidateService
    {
        Task CreateCandidateAsync(CandidateInformationalModel candidateToCreate);
        Task<CandidateInformationalModel> GetCandidateByIdAsync(Guid id);
        Task<List<CandidateInformationalModel>> GetCandidatesBySeniorityAsync(Seniority seniority);
        Task<List<CandidateInformationalModel>> GetAllCandidatesAsync();
        Task UpdateCandidateAsync(CandidateModel candidate);
        Task DeleteCandidateAsync(Guid id);
    }
}
