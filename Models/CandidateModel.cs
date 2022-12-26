namespace AttestationProject.Models
{
    public class CandidateModel : CandidateInformationalModel
    {

        // Sperating visible and internal information
        // Allowing for automatic Guid generation during the inicialization of the CandidateModel
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
