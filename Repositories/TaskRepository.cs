namespace AttestationProject.Repositories
{
    public class TaskRepository : Interfaces.Repositories.ITaskRepository
    {
        public async Task<TableClient> GetTableClientAsync()
        {
            TableServiceClient skillTableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            TableClient skillTableClient = skillTableServiceClient.GetTableClient(tableName: "Skill");
            await skillTableClient.CreateIfNotExistsAsync();
            return skillTableClient;
        }

        public async Task<AsyncPageable<SkillRecord>> GetTaskByIdAsync(string id)
        {
            TableClient skillTableClient = await GetTableClientAsync();
            AsyncPageable<SkillRecord> skillRecords = skillTableClient.QueryAsync<SkillRecord>(x => x.RowKey == id);
            return skillRecords;
        }

    }
}
