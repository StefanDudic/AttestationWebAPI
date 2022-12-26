namespace AttestationProject.Models
{
    public class TaskInformationalModel
    {

        // Allowing all properties to be nullable to enable partial updates of table entries based on enetered values in a form (if value is null that property is not updated) 
        // Custom validation will be implemented as a part of "CreateCandidate" function in the service layer

        public string? Title { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Seniority? Level { get; set; }

        public string? Description { get; set; }

        public string? Code { get; set; }
    }
}
