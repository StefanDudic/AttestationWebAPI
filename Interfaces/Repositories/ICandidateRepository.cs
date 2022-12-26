namespace AttestationProject.Interfaces.Repositories
{
    public interface ICandidateRepository
    {
        Task CreateCandidateAsync(CandidateRecord candidateToCreate);
        Task<AsyncPageable<CandidateRecord>> GetCandidateByIdAsync(Guid id);
        Task<AsyncPageable<CandidateRecord>> GetCandidatesBySeniorityAsync(Seniority seniority);
        Task<AsyncPageable<CandidateRecord>> GetAllCandidatesAsync();
        Task UpdateCandidateAsync(CandidateRecord candidateRecordToUpdate);
        Task DeleteCandidateAsync(Guid id);
    }
}
