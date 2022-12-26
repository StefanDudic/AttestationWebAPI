using Microsoft.AspNetCore.Mvc;

namespace AttestationProject.Models
{
    public class SkillInformationalModel
    {

        // Allowing all properties to be nullable to enable partial updates of table entries based on enetered values in a form (if value is null that property is not updated) 
        // Custom validation will be implemented as a part of "CreateSkill" function in the service layer

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SkillName? Name { get; set; }

        public List<TaskInformationalModel>? Tasks { get; set; }
    }
}
