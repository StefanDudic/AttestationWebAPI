using AttestationProject.Models;
using System.Collections.Generic;

namespace AttestationProject.Services
{
    public class TaskService : ITaskService
    {
        private IValidationDictionary _validatonDictionary;
        private ITaskRepository _repository;
        private ISkillRepository _skillRepository;

        public TaskService (IValidationDictionary validatonDictionary, ITaskRepository repository, ISkillRepository skillRepository)
        {
            _validatonDictionary = validatonDictionary;
            _repository = repository;
            _skillRepository = skillRepository;
        }

        #region Internal functions
        internal async Task<bool> ValidateTaskAsync(TaskInformationalModel taskForValidation)
        {
            if (taskForValidation.Title is null)
                _validatonDictionary.AddError("Title", "Title is required.");
            if (taskForValidation.Level is null)
                _validatonDictionary.AddError("Level", "Level is required.");
            return _validatonDictionary.IsValid;
        }

        #endregion

        #region CRUD operations

        public async Task CreateTaskAsync(TaskInformationalModel taskToCreate, SkillName skillName)
        {
            // Validates task entry
            if (!await ValidateTaskAsync(taskToCreate))
                throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.BadRequest);
            // Checking if the skill, we want to add the task to, exists
            List<SkillRecord> existCheck = await Mapper.CreateSkillListFromQueryResponse(await _skillRepository.GetSkillByNameAsync(skillName.ToString()));
            if (existCheck.Count == 0)
            {
                throw new HttpRequestException("Skill not found.", null, HttpStatusCode.NotFound);
            }
            // Checking if the skill exists in Table Storage without a task and if so, removes that entry and inserts it back with the added task
            if (existCheck.First().Task is null)
            {
                await _skillRepository.DeleteSkillAsync(new List<SkillRecord>() { existCheck[0] });
            }
            else
            {
                // Checking if task with the same title already exists under the same skill
                foreach (SkillRecord record in existCheck)
                {
                    if (taskToCreate.Title + "\"" == record.Task.Substring(10, taskToCreate.Title.Length + 1))
                        throw new HttpRequestException("Task alredy exists.", null, HttpStatusCode.Conflict);
                }
            }
            // Creates a record to be added to the Table Storage
            SkillRecord skillRecord = Mapper.CreateSkillRecordFromTaskInformationalModel(taskToCreate, existCheck[0].PartitionKey, existCheck[0].RowKey.Substring(0, 36)); 
            await _skillRepository.CreateSkillAsync(skillRecord);
        }

        #endregion
    }
}
