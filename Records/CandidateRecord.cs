
namespace AttestationProject.Records
{
    //Azure Table Storage entity
    public record CandidateRecord : ITableEntity
    {
        //Seniority as it provides the ability to easily search logical partitions in indexed row
        public string PartitionKey { get; set; }
        
        //Id
        public string RowKey { get; set; } 

        public string Name { get; set; }

        public string Surname { get; set; }

        // Allowing phone nuber to be null as it can possibly be provided later by the candidate and updated
        public string? PhoneNumber { get; set; } 

        public ETag ETag { get; set; } = default!; 

        public DateTimeOffset? Timestamp { get; set; } = default!; 
    }
}
