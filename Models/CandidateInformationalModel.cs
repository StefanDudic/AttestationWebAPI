namespace AttestationProject.Models
{
    // Candidate informational model is used to show information about the candi
    public class CandidateInformationalModel
    {

        // Allowing all properties to be nullable to enable partial updates of table entries based on enetered values in a form (if value is null that property is not updated) 
        // Custom validation is implemented as a part of "CreateCandidate" function in the service layer
        [RegularExpression(@"^[a-zA-z,.'-ćčžđšČĆŽĐŠ]+$", ErrorMessage = "Name is not in a valid format.")]
        public string? Name { get; set; }
        [RegularExpression(@"^[a-zA-z,.'-ćčžđšČĆŽĐŠ]+$", ErrorMessage = "Surname is not in a valid format.")]
        public string? Surname { get; set; }

        [RegularExpression(@"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{3,4})(?: *x(\d+))?\s*$", ErrorMessage = "Phone number is not in a valid format.")]
        public string? PhoneNumber { get; set; } = default;

        // Allows Seniority enum type to be shown as value in a response body 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Seniority? Seniority { get; set; }
    }
}
