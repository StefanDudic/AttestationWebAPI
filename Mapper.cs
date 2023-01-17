using System.Text.Json;

namespace AttestationProject
{
    // Mapper class provides conversion functions used thorught the project
    public static class Mapper
    {
        #region Candidate
        public static CandidateModel CreateCandidateModel(CandidateInformationalModel candidateInformationalModel)
        {
            CandidateModel candidateModel = new();
            candidateModel.Name = candidateInformationalModel.Name;
            candidateModel.Surname = candidateInformationalModel.Surname;
            candidateModel.PhoneNumber = candidateInformationalModel.PhoneNumber;
            candidateModel.Seniority = candidateInformationalModel.Seniority;
            return candidateModel;
        }

        // Returns full candidate model with Id [NOT USED]
        public static CandidateModel CandidateRecordToModel(CandidateRecord candidateRecord)
        {
            CandidateModel candidateModel = new();
            Guid id;
            if (!Guid.TryParse(candidateRecord.RowKey, out id))
            {
                throw new GuidNotValidException(candidateRecord.RowKey);
            }
            candidateModel.Id = id;
            candidateModel.Name = candidateRecord.Name;
            candidateModel.Surname = candidateRecord.Surname;
            candidateModel.PhoneNumber = candidateRecord.PhoneNumber;
            Seniority seniority;
            if (!Enum.TryParse<Seniority>(candidateRecord.PartitionKey, out seniority))
            {
                throw new EnumNotValidException(candidateRecord.PartitionKey);
            }
            candidateModel.Seniority = seniority;
            return candidateModel;
        }

        // Returns informational candidate model without Id
        public static CandidateInformationalModel CandidateRecordToInformationalModel(CandidateRecord candidateRecord)
        {
            CandidateInformationalModel candidateModel = new();
            candidateModel.Name = candidateRecord.Name;
            candidateModel.Surname = candidateRecord.Surname;
            candidateModel.PhoneNumber = candidateRecord.PhoneNumber;
            Seniority seniority;
            if (!Enum.TryParse<Seniority>(candidateRecord.PartitionKey, out seniority))
            {
                throw new EnumNotValidException(candidateRecord.PartitionKey);
            }
            candidateModel.Seniority = seniority;
            return candidateModel;
        }

        // Returns a record to be added to the Azure storage Table
        public static CandidateRecord CandidateModelToRecord(CandidateModel candidateModel)
        {
            CandidateRecord candidateRecord = new();
            candidateRecord.PartitionKey = candidateModel.Seniority.ToString();
            candidateRecord.RowKey = candidateModel.Id.ToString();
            candidateRecord.Name = candidateModel.Name;
            candidateRecord.Surname = candidateModel.Surname;
            candidateRecord.PhoneNumber = candidateModel.PhoneNumber;
            return candidateRecord;
        }

        // In order to achive asynchronous functionality, this helper function is used to convert AsyncPageable response form the server to the list of model entityes we need to return
        public static async Task<List<CandidateInformationalModel>> CreateCandidateListFromQueryResponse(AsyncPageable<CandidateRecord> candidates)
        {
            List<CandidateInformationalModel> candidatesList = new();
            await foreach (CandidateRecord candidate in candidates)
            {
                candidatesList.Add(Mapper.CandidateRecordToInformationalModel(candidate));
            }
            return candidatesList;
        }

        #endregion

        #region Skill

        // Returns full skill model with generated Id
        public static SkillModel CreateSkillModel(SkillInformationalModel skillInformationalModel)
        {
            SkillModel skillModel = new();
            skillModel.Name = skillInformationalModel.Name;
            // Skill can be added without tasks, following condition checks that, and adjusts the assignment of Tasks property
            if (skillInformationalModel.Tasks == null || skillInformationalModel.Tasks.Count == 0)
                skillModel.Tasks = null;
            else 
            skillModel.Tasks = skillInformationalModel.Tasks.ToList<TaskInformationalModel>();
            return skillModel;
        }

        // Returns skill informational model (without Id value), to be presented as a response to get requests. 
        public static SkillInformationalModel SkillRecordToInformationalModel(SkillRecord skillRecord)
        {
            SkillInformationalModel skillModel = new();
            SkillName skillName;
            if (!Enum.TryParse<SkillName>(skillRecord.PartitionKey, out skillName))
            {
                throw new EnumNotValidException(skillRecord.PartitionKey);
            }
            skillModel.Name = skillName;
            skillModel.Tasks = new List<TaskInformationalModel>();
            if (skillRecord.Task is not null) 
            {
                skillModel.Tasks.Add(JsonSerializer.Deserialize<TaskInformationalModel>(skillRecord.Task));
            }
            return skillModel;
        }

        // Returns aggregated skill entry with all tasks that are stored as separate rows in Skill table for the same skill
        public static SkillInformationalModel SkillRecordsToInformationalModel(List<SkillRecord> skillRecords)
        {
            SkillInformationalModel skillModel = new();
            SkillName skillName;
            if (!Enum.TryParse<SkillName>(skillRecords[0].PartitionKey, out skillName))
            {
                throw new EnumNotValidException(skillRecords[0].PartitionKey);
            }
            skillModel.Name = skillName;
            skillModel.Tasks = new List<TaskInformationalModel>();
            foreach (SkillRecord skillRecord in skillRecords)
            {
                skillModel.Tasks.Add(JsonSerializer.Deserialize<TaskInformationalModel>(skillRecord.Task));
            }
            return skillModel;
        }

        // Returns a skill record list to be added to the Azure storage Table for the passed skill model with one or more task entities
        public static List<SkillRecord> SkillModelToRecordList(SkillModel skillModel)
        {
            List<SkillRecord> skillRecordList = new();
            foreach (TaskInformationalModel task in skillModel.Tasks)
            {
                SkillRecord skillRecord = new();
                skillRecord.PartitionKey = skillModel.Name.ToString();
                skillRecord.RowKey = skillModel.Id + "_" + Guid.NewGuid().ToString();
                skillRecord.Task = JsonSerializer.Serialize<TaskInformationalModel>(task);
                skillRecordList.Add(skillRecord);
            }
            return skillRecordList;
        }

        // Returns a skill record list to be added to the Azure storage Table for the passed skill model without task entities
        public static SkillRecord SkillModelToRecord(SkillModel skillModel)
        {
            SkillRecord skillRecord = new();
            skillRecord.PartitionKey = skillModel.Name.ToString();
            skillRecord.RowKey = skillModel.Id.ToString();
            return skillRecord;
        }

        // In order to achive asynchronous functionality, this helper function is used to convert AsyncPageable response form the server to the list of model entityes we need to return
        public static async Task<List<SkillRecord>> CreateSkillListFromQueryResponse(AsyncPageable<SkillRecord> skills)
        {
            List<SkillRecord> skillsList = new();
            await foreach (SkillRecord skill in skills)
            {
                skillsList.Add(skill);
            }
            return skillsList;
        }

        
        #endregion

        #region Task

        // Returns a skill recored to be added to the table storage for a passed task values and a selected skill
        public static SkillRecord CreateSkillRecordFromTaskInformationalModel(TaskInformationalModel taskInformationalModel, string skillName, string skillId)
        {
            SkillRecord skillRecord = new();
            skillRecord.PartitionKey = skillName;
            skillRecord.RowKey = skillId + "_" + Guid.NewGuid();
            if (taskInformationalModel.Description is null)
                taskInformationalModel.Description = "";
            if (taskInformationalModel.Code is null)
                taskInformationalModel.Code = "";
            skillRecord.Task = JsonSerializer.Serialize<TaskInformationalModel>(taskInformationalModel);
            return skillRecord;
        }

        // Returns a single task entity from the query response
        public static async Task<TaskInformationalModel> CreateTaskInformationalModelFromQueryResponse(AsyncPageable<SkillRecord> skills)
        {
            SkillRecord skillRecord =new();
            await foreach (SkillRecord skill in skills)
            {
                skillRecord = skill;
            }
            // Setting a value to be returned as null if response contains no elements
            TaskInformationalModel taskInformationalModel = null;
            if(skillRecord.Task != null)
            taskInformationalModel = JsonSerializer.Deserialize<TaskInformationalModel>(skillRecord.Task);
            return taskInformationalModel;
        }
        public static TaskInformationalModel CreateTaskInformationalModelFromSkillRecord (SkillRecord skillRecord)
        {
            TaskInformationalModel taskInformationalModel = null;
            taskInformationalModel = JsonSerializer.Deserialize<TaskInformationalModel>(skillRecord.Task);
            return taskInformationalModel;
        }
        #endregion
    }
}
