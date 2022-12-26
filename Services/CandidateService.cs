using Microsoft.AspNetCore.Routing.Matching;

namespace AttestationProject.Services
{
    //Serive layar provides data validation and converson of entities to appropriate format
    public class CandidateService : ICandidateService
    {
        #region Initialization 

        // Provides functionality for storing and reading validation data
        private IValidationDictionary _validatonDictionary; 
        private ICandidateRepository _repository;

        public CandidateService(IValidationDictionary validatonDictionary, ICandidateRepository repository)
        {
            _validatonDictionary = validatonDictionary;
            _repository = repository;
        }

        #endregion

        #region Internal functions

        // Validates that the entered values are in the correct format for conversion to a Table Storage entity
        internal bool ValidateCandidate(CandidateInformationalModel candidateForValidation)
        {
            if (candidateForValidation.Name is null)
                _validatonDictionary.AddError("Name", "Name is required.");
            if (candidateForValidation.Surname is null)
                _validatonDictionary.AddError("Surname", "Surname is required.");
            if(candidateForValidation.Seniority is null)
            {
                _validatonDictionary.AddError("Seniority", "Seniority is required.");
            }
            return _validatonDictionary.IsValid;
        }

        // Returns the enumerator pointed to the first entry in AsyncPageable response list
        internal async Task<T> FirstOrDefaultAsync<T>(IAsyncEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();
            return enumerator.Current;
        }

        #endregion

        #region CRUD operations

        public async Task CreateCandidateAsync(CandidateInformationalModel candidateInformationalModel)
        {
            if (!ValidateCandidate(candidateInformationalModel))
                throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.BadRequest);
            // Adding an auto-generated Id property by performing conversion to the CandidateModel
            CandidateModel candidateToCreate = Mapper.CreateCandidateModel(candidateInformationalModel);
            CandidateRecord candidateRecord = Mapper.CandidateModelToRecord(candidateToCreate);
            await _repository.CreateCandidateAsync(candidateRecord);
        }

        public async Task<CandidateInformationalModel> GetCandidateByIdAsync(Guid id)
        {
            AsyncPageable<CandidateRecord> candidates = await _repository.GetCandidateByIdAsync(id);
            if(FirstOrDefaultAsync(candidates).Result == null)
            {
                throw new HttpRequestException("Candidate not found.", null, HttpStatusCode.NotFound);
            }
            CandidateInformationalModel candidate = Mapper.CandidateRecordToInformationalModel(FirstOrDefaultAsync(candidates).Result);
            return candidate;
        }

        public async Task<List<CandidateInformationalModel>> GetCandidatesBySeniorityAsync(Seniority seniority)
        {
            List<CandidateInformationalModel> response = await Mapper.CreateCandidateListFromQueryResponse(await _repository.GetCandidatesBySeniorityAsync(seniority));
            if(response.Count == 0)
            {
                throw new HttpRequestException("Candidates not found.", null, HttpStatusCode.NotFound);
            }
            return response;
        }

        public async Task<List<CandidateInformationalModel>> GetAllCandidatesAsync()
        {
            return await Mapper.CreateCandidateListFromQueryResponse(await _repository.GetAllCandidatesAsync());
        }

        public async Task UpdateCandidateAsync(CandidateModel candidate)
        {
            // Checking if values are entered
            if (candidate.Name is null && candidate.Surname is null && candidate.PhoneNumber is null && candidate.Seniority is null)
            {
                _validatonDictionary.AddError("InvalidValues", "No values to update");
                throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.BadRequest);
            }
                AsyncPageable<CandidateRecord> resault = await _repository.GetCandidateByIdAsync(candidate.Id);
            if (FirstOrDefaultAsync(resault).Result == null)
            {
                throw new HttpRequestException("Candidate not found.", null, HttpStatusCode.NotFound);
            }
            CandidateRecord candidateRecordToUpdate = await FirstOrDefaultAsync(resault);
                if (candidate.Name is not null)
                    candidateRecordToUpdate.Name = candidate.Name;
                if (candidate.Surname is not null)
                    candidateRecordToUpdate.Surname = candidate.Surname;
                if (candidate.PhoneNumber is not null)
                    if (candidate.PhoneNumber != "000000000")
                    {
                        candidateRecordToUpdate.PhoneNumber = candidate.PhoneNumber;
                    }
                    else
                    {
                        candidateRecordToUpdate.PhoneNumber = null;
                    }
                if (candidate.Seniority is null)
                {
                    await _repository.UpdateCandidateAsync(candidateRecordToUpdate);
                }
                else if (candidateRecordToUpdate.PartitionKey == candidate.Seniority.ToString())
                {
                    await _repository.UpdateCandidateAsync(candidateRecordToUpdate);
                }
                else
                {
                    candidateRecordToUpdate.PartitionKey = candidate.Seniority.ToString();
                    await _repository.DeleteCandidateAsync(candidate.Id);
                    await _repository.CreateCandidateAsync(candidateRecordToUpdate);
                }
        }

        public async Task DeleteCandidateAsync(Guid id)
        {
            await _repository.DeleteCandidateAsync(id);
        }

        #endregion

    }
}
