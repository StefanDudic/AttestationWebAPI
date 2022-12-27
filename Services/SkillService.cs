using Microsoft.Extensions.Azure;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;

namespace AttestationProject.Services
{
    public class SkillService : ISkillService
    {
        #region Initialization 

        // Provides functionality for storing and reading validation data
        private IValidationDictionary _validatonDictionary;
        private ISkillRepository _repository;

        public SkillService(IValidationDictionary validatonDictionary, ISkillRepository repository)
        {
            _validatonDictionary = validatonDictionary;
            _repository = repository;
        }

        #endregion

        #region Internal functions

        // Validates that the entered values are in the correct format for conversion to a Table Storage entity
        internal async Task<bool> ValidateSkillAsync(SkillInformationalModel skillForValidation)
        {
            if (skillForValidation.Name is null)
                _validatonDictionary.AddError("Name", "Name is required.");
            SkillName name = (SkillName)skillForValidation.Name;
            try
            {
                SkillInformationalModel skillExists = await GetSkillByNameAsync(name);
            }
            catch
            (Exception ex)
            {
                return _validatonDictionary.IsValid;
            }
            _validatonDictionary.AddError("Skill exists", "Skill already exists.");
            return _validatonDictionary.IsValid;
        }

        #endregion

        #region CRUD operations

        public async Task CreateSkillAsync(SkillInformationalModel skillInformationalModel)
        {
            // Validates skill entry
            if (!await ValidateSkillAsync(skillInformationalModel))
                throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.BadRequest);

            // Adding an auto-generated Id property by performing conversion to the SkillModel
            SkillModel skillToCreate = Mapper.CreateSkillModel(skillInformationalModel);

            // Checking if the any tasks have been passed, if so, we need to create multiple skill entries 
            // Need to add task validation after it has been implemented
            if (skillToCreate.Tasks is null || skillToCreate.Tasks.Count == 0)
            {
                SkillRecord skillRecord = Mapper.SkillModelToRecord(skillToCreate);
                await _repository.CreateSkillAsync(skillRecord);
            }
            else
            {
                foreach (SkillRecord skillRecordFromTheList in Mapper.SkillModelToRecordList(skillToCreate))
                {
                    await _repository.CreateSkillAsync(skillRecordFromTheList);
                }
            }

        }

        public async Task<SkillInformationalModel> GetSkillByIdAsync(Guid id)
        {
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByIdAsync(id.ToString()));
            if (response.Count == 0)
            {
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);
            }
            SkillInformationalModel skill = Mapper.SkillRecordsToInformationalModel(response);
            return skill;
        }

        public async Task<SkillInformationalModel> GetSingleSkillByIdAsync(Guid id)
        {
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByIdAsync(id.ToString()));
            if (response.Count == 0)
            {
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);
            }
            SkillInformationalModel skill = Mapper.SkillRecordsToInformationalModel(response);
            return skill;
        }

        public async Task<SkillInformationalModel> GetSkillByNameAsync(SkillName skillName)
        {
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByNameAsync(skillName.ToString()));
            if (response.Count == 0)
            {
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);
            }
            SkillInformationalModel skill = Mapper.SkillRecordsToInformationalModel(response);
            return skill;
        }

        public async Task<List<SkillInformationalModel>> GetAllSkillsAsync()
        {
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetAllSkillsAsync());
            if (response.Count == 0)
            {
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);
            }
            // Storing skills in dictionary for aggregation of task records under the same skill into the skill model to be returned
            Dictionary<string, SkillInformationalModel> sorter = new Dictionary<string, SkillInformationalModel>();
            foreach (SkillRecord record in response)
            {
                if (sorter.ContainsKey(record.PartitionKey))
                {
                    sorter[record.PartitionKey].Tasks.Add(JsonSerializer.Deserialize<TaskInformationalModel>(record.Task));
                }
                else
                {
                    sorter.Add(record.PartitionKey, Mapper.SkillRecordToInformationalModel(record));
                }
            }
            // Adding skills to the list to be returned as a response
            List<SkillInformationalModel> skills = new List<SkillInformationalModel>();
            foreach (KeyValuePair<string, SkillInformationalModel> entry in sorter)
            {
                skills.Add(entry.Value);
            }
            return skills;
        }

        public async Task UpdateSkillAsync(SkillUpdateModel skill)
        {
            // Checking if skill exists before the update
            List<SkillRecord> existCheck = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByIdAsync(skill.Id.ToString()));
            if (existCheck.Count == 0)
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);

            // If a name update is requested for the passed skull id we need to remove skill entries first and add them with the updated name, as a name is the key value in Table Storage and can't be updated
            if (existCheck[0].PartitionKey != skill.Name.ToString())
            {
                // We first need to get the same id of already existing skill under the same name as passed updated name value, if exists, to use for the new inserts
                string skillid;
                List<SkillRecord> skillIdCheck = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByNameAsync(skill.Name.ToString()));
                if(skillIdCheck.Count != 0)
                {
                    // Get only skill id from the composit skillId_taskId RowKey.
                    skillid= skillIdCheck[0].RowKey.Substring(0, 36);
                }
                else
                {
                    // If a skill with the same name doesn't exist in the table, we can use the same id
                    skillid = skill.Id.ToString();
                }
                await DeleteSkillAsync(skill.Id.ToString());

                foreach (SkillRecord skillRecord in existCheck)
                {
                    skillRecord.PartitionKey = skill.Name.ToString();
                    // Replacing just the skill id without changing task id for all entries
                    skillRecord.RowKey = skillRecord.RowKey.Replace(skillRecord.RowKey.Substring(0, 36), skillid);
                    await _repository.CreateSkillAsync(skillRecord);
                }
            }
            // If any task information is passed we need to either insert it or update it
            foreach (TaskModel task in skill.Tasks)
            {
                // Checking if the task already exists for the passed id and needs to be updated. If not, we need to insert it as a new skill entry
                List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByIdAsync(skill.Id + "_" + task.Id));
                if (response.Count == 0)
                {
                    // Checking if the skill exists in Table Storage without a task and if so, updates that entry to add the first task in the list
                    if (existCheck.First().Task is null)
                    {
                        existCheck.First().Task = "updated";
                        await DeleteSkillAsync(skill.Id.ToString());
                    }
                    SkillRecord skillRecord = new();
                    skillRecord.PartitionKey = skill.Name.ToString();
                    // If default Guid is passed we need to create a new id for the task
                    if (task.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        skillRecord.RowKey = skill.Id + "_" + Guid.NewGuid();
                    }
                    else
                    {
                        skillRecord.RowKey = skill.Id + "_" + task.Id.ToString();
                    }
                    skillRecord.Task = JsonSerializer.Serialize<TaskInformationalModel>(task);
                    await _repository.CreateSkillAsync(skillRecord);
                }  else 
                {
                    // Updating task information for the passed task id by editing the JSON string to change the values
                    SkillRecord skillRecordToUpdate = response.First();
                    SkillInformationalModel skillModelToUpdate = Mapper.SkillRecordToInformationalModel(response.First());
                    if (task.Title != "" && task.Title is not null)
                        skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Title\":\"" + skillModelToUpdate.Tasks[0].Title, "\"Title\":\"" + task.Title);
                    if (task.Level is not null)
                        skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Level\":\"" + skillModelToUpdate.Tasks[0].Level.ToString(), "\"Level\":\"" + task.Level.ToString());
                    if (task.Description is not null)
                        skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Description\":\"" + skillModelToUpdate.Tasks[0].Description, "\"Description\":\"" + task.Description);
                    if (task.Code is not null)
                        skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Code\":\"" + skillModelToUpdate.Tasks[0].Code, "\"Code\":\"" + task.Code);
                    // Need to add Task valudation after it has been implemented
                    await _repository.UpdateSkillAsync(skillRecordToUpdate);
                }
            }
        }

        public async Task DeleteSkillAsync(string param)
        {
            Guid g;
            // Both skill id and name can be passed as a parameter.The following condition determines whether to call for a function with id or a name value
            if (Guid.TryParse(param, out g))
            {
                // Checking if skill exists
                List<SkillRecord> skillRecordResponse = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByIdAsync(g.ToString()));
                if (skillRecordResponse.Count == 0)
                {
                    throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.NotFound);
                }
                    await _repository.DeleteSkillAsync(skillRecordResponse);
            }
            else
            {
                // Checking if skill exists
                List<SkillRecord> skillRecordResponse = await Mapper.CreateSkillListFromQueryResponse(await _repository.GetSkillByNameAsync(param));
                if (skillRecordResponse.Count == 0)
                {
                    throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.NotFound);
                }
                await _repository.DeleteSkillAsync(skillRecordResponse);
            }
        }

        #endregion
    }
}
