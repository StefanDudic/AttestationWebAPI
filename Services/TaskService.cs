using System.Threading.Tasks;

namespace AttestationProject.Services
{
    public class TaskService : ITaskService
    {
        private IValidationDictionary _validatonDictionary;
        private ITaskRepository _repository;
        private ISkillRepository _skillRepository;

        public TaskService(IValidationDictionary validatonDictionary, ITaskRepository repository, ISkillRepository skillRepository)
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
        internal string reverseString(string str)
        {
            if (str.Length <= 1) return str;
            else return reverseString(str.Substring(1)) + str[0];
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

        public async Task<TaskInformationalModel> GetTaskByIdAsync(Guid skillId, Guid taskId)
        {
            // We need to pass aggregated id that would match RowKey value in Table storage in skillId_taskId format
            TaskInformationalModel taskInformationalModel = await Mapper.CreateTaskInformationalModelFromQueryResponse(await _repository.GetTaskByIdAsync(skillId.ToString() + "_" + taskId.ToString()));
            // Validation if task exists
            if (taskInformationalModel is null)
            {
                throw new HttpRequestException("Task not found.", null, HttpStatusCode.NotFound);
            }
            return taskInformationalModel;
        }

        public async Task<List<TaskInformationalModel>> GetAllTasksAsync()
        {
            List<TaskInformationalModel> taskInformationalModels = new List<TaskInformationalModel>();
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _skillRepository.GetAllSkillsAsync());
            if (response.Count == 0)
            {
                throw new HttpRequestException("Task not found.", null, HttpStatusCode.NotFound);
            }
            foreach (SkillRecord skillRecord in response)
            {
                taskInformationalModels.Add(Mapper.CreateTaskInformationalModelFromSkillRecord(skillRecord));
            }
            return taskInformationalModels;
        }

        public async Task UpdateTaskAsync(TaskModel task, Guid skillId)
        {
            // Checking if task exists before the update
            List<SkillRecord> existCheck = await Mapper.CreateSkillListFromQueryResponse(await _skillRepository.GetSkillByIdAsync(skillId.ToString() + "_" + task.Id.ToString()));
            if (existCheck.Count == 0)
                throw new HttpRequestException("Task not found.", null, HttpStatusCode.NotFound);

            SkillRecord skillRecordToUpdate = existCheck.First<SkillRecord>();
            // Updating task information for the passed task id by editing the JSON string to change the values
            SkillInformationalModel skillModelToUpdate = Mapper.SkillRecordToInformationalModel(skillRecordToUpdate);
            if (task.Title != "" && task.Title is not null)
                skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Title\":\"" + skillModelToUpdate.Tasks[0].Title, "\"Title\":\"" + task.Title);
            if (task.Level is not null)
                skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Level\":\"" + skillModelToUpdate.Tasks[0].Level.ToString(), "\"Level\":\"" + task.Level.ToString());
            if (task.Description is not null)
                skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Description\":\"" + skillModelToUpdate.Tasks[0].Description, "\"Description\":\"" + task.Description);
            if (task.Code is not null)
                skillRecordToUpdate.Task = skillRecordToUpdate.Task.Replace("\"Code\":\"" + skillModelToUpdate.Tasks[0].Code, "\"Code\":\"" + task.Code);
            // Need to add Task valudation after it has been implemented
            await _skillRepository.UpdateSkillAsync(skillRecordToUpdate);
        }
        public async Task DeleteTaskAsync(Guid skillId, Guid taskId)
        {
            // Checking if task exists
            List<SkillRecord> taskRecordToDelete = await Mapper.CreateSkillListFromQueryResponse(await _skillRepository.GetSkillByIdAsync(skillId.ToString() + "_" + taskId.ToString()));
            if(taskRecordToDelete.Count == 0)
            {
                throw new HttpRequestException(_validatonDictionary.Errors(), null, HttpStatusCode.NotFound);
            }
            // Checking if we are deleting the last task entry, if so we need to leave the skill entry in the Table storage without a task
            List<SkillRecord> response = await Mapper.CreateSkillListFromQueryResponse(await _skillRepository.GetSkillByIdAsync(skillId.ToString()));
            if (response.Count > 1)
            {
                await _skillRepository.DeleteSkillAsync(taskRecordToDelete);
            }
            else {
                await _skillRepository.DeleteSkillAsync(taskRecordToDelete);
                taskRecordToDelete[0].Task = null;
                taskRecordToDelete[0].RowKey = skillId.ToString();
                await _skillRepository.CreateSkillAsync(taskRecordToDelete.First());
            }
        }

        #endregion
    }
}


