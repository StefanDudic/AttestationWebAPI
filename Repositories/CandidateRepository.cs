namespace AttestationProject.Repositories
{
    public class CandidateRepository : ICandidateRepository, ITableClient
    {
        // Repository class provides Azure Table Storage connection and data manipulation functionality
        public async Task<TableClient> GetTableClientAsync()
        {
            TableServiceClient candidateTableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            TableClient candidateTableClient = candidateTableServiceClient.GetTableClient(tableName: "Candidate");
            await candidateTableClient.CreateIfNotExistsAsync();
            return candidateTableClient;
        }
        public async Task CreateCandidateAsync(CandidateRecord candidateToCreate)
        {
            TableClient candidateTableClient = await GetTableClientAsync();
            await candidateTableClient.AddEntityAsync<CandidateRecord>(candidateToCreate);            
        }

        public async Task<AsyncPageable<CandidateRecord>> GetCandidateByIdAsync(Guid id)
        {
            TableClient candidateTableClient = await GetTableClientAsync();
            AsyncPageable<CandidateRecord> candidateRecord = candidateTableClient.QueryAsync<CandidateRecord>(x => x.RowKey == id.ToString());
            return candidateRecord;
        }

        public async Task<AsyncPageable<CandidateRecord>> GetCandidatesBySeniorityAsync(Seniority seniority)
        {
            TableClient candidateTableClient = await GetTableClientAsync();
            string seniorityValue = seniority.ToString();
            AsyncPageable<CandidateRecord> candidateRecords = candidateTableClient.QueryAsync<CandidateRecord>( x => x.PartitionKey == seniorityValue);
            return candidateRecords;
        }

        public async Task<AsyncPageable<CandidateRecord>> GetAllCandidatesAsync()
        {
            TableClient candidateTableClient = await GetTableClientAsync();
            AsyncPageable<CandidateRecord> candidateRecords =  candidateTableClient.QueryAsync<CandidateRecord>();
            return candidateRecords;

        }

        public async Task UpdateCandidateAsync(CandidateRecord candidateRecordToUpdate)
        {
            TableClient candidateTableClient = await  GetTableClientAsync();
            await candidateTableClient.UpdateEntityAsync<CandidateRecord>(candidateRecordToUpdate, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteCandidateAsync(Guid id)
        {
            TableClient candidateTableClient = await GetTableClientAsync();
            var candidateEntryResponse = candidateTableClient.Query<CandidateRecord>(x => x.RowKey == id.ToString());
            CandidateRecord candidateRecord = candidateEntryResponse.First();
            await candidateTableClient.DeleteEntityAsync(candidateRecord.PartitionKey, candidateRecord.RowKey);
        }
    }
}
