namespace AttestationProject.Models
{
    public class SkillModel : SkillInformationalModel
    {

        // Sperating visible and internal information
        // Allowing for automatic Guid generation during the inicialization of the SkillModel
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; } = Guid.NewGuid();

    }
}
