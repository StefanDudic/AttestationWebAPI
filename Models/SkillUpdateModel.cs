namespace AttestationProject.Models
{
    public class SkillUpdateModel
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SkillName? Name { get; set; }

        public List<TaskModel>? Tasks { get; set; }
    }
}

