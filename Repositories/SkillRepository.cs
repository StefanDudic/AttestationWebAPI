using System.Data.Common;

namespace AttestationProject.Repositories
{
    public class SkillRepository : ISkillRepository, ITableClient
    {
        // Repository class provides Azure Table Storage connection and data manipulation functionality
        public async Task<TableClient> GetTableClientAsync()
        {
            TableServiceClient skillTableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            TableClient skillTableClient = skillTableServiceClient.GetTableClient(tableName: "Skill");
            await skillTableClient.CreateIfNotExistsAsync();
            return skillTableClient;
        }
        public async Task CreateSkillAsync(SkillRecord skillToCreate)
        {
            TableClient skillTableClient = await GetTableClientAsync();
            await skillTableClient.AddEntityAsync<SkillRecord>(skillToCreate);
        }

        public async Task<AsyncPageable<SkillRecord>> GetSkillByIdAsync(string id) 
        {
            TableClient skillTableClient = await GetTableClientAsync();
            // Filtering by skill Id in composit skillId_taskId Row Key property. Table Storage does not support "Contains" LINQ operatior
            var rowKeyRangeEnd = id.ToString().Substring(0, id.ToString().Length - 1) + (char)(id.ToString().Last() + 1);
            AsyncPageable<SkillRecord> skillRecords = skillTableClient.QueryAsync<SkillRecord>(x => x.RowKey.CompareTo(id) >=0 && x.RowKey.CompareTo(rowKeyRangeEnd) <0); 
            return skillRecords;
        }

        public async Task<AsyncPageable<SkillRecord>> GetSkillByNameAsync(string skillName)
        {
            TableClient skillTableClient = await GetTableClientAsync();
            AsyncPageable<SkillRecord> skillRecords = skillTableClient.QueryAsync<SkillRecord>(x => x.PartitionKey == skillName);
            return skillRecords;
        }

        public async Task<AsyncPageable<SkillRecord>> GetAllSkillsAsync()
        {
            TableClient skillTableClient = await GetTableClientAsync();
            AsyncPageable<SkillRecord> skillRecords = skillTableClient.QueryAsync<SkillRecord>();
            return skillRecords;

        }

        public async Task UpdateSkillAsync(SkillRecord skillRecordToUpdate)
        {
            TableClient skillTableClient = await GetTableClientAsync();
            await skillTableClient.UpdateEntityAsync<SkillRecord>(skillRecordToUpdate, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteSkillAsync(List<SkillRecord> skillRecordResponse)
        {
            TableClient skillTableClient = await GetTableClientAsync();
            foreach (SkillRecord record in skillRecordResponse)
            {
                await skillTableClient.DeleteEntityAsync(record.PartitionKey, record.RowKey);
            }
        }   
    }
}
