namespace AttestationProject.Records
{
    //Azure Table Storage entity
    public record SkillRecord : ITableEntity
    {
        //Name as it provides the ability to easily search logical partitions in indexed row
        public string PartitionKey { get; set; }

        //SkillId_TaskId - Implementing "Intra-partition secondary index" pattern to allow for update consistency using entity group transactions (EGTs)
        public string RowKey { get; set; } 

        //Task entity in JSON format
        public string? Task { get; set; } 

        public ETag ETag { get; set; } = default!; 

        public DateTimeOffset? Timestamp { get; set; } = default!; 
    }
}
