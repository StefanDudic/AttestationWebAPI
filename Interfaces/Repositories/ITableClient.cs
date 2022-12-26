namespace AttestationProject.Interfaces.Repositories
{
    public interface ITableClient
    {
        Task<TableClient> GetTableClientAsync();
    }
}
