namespace AttestationProject.Services
{
    public class TaskService : ITaskService
    {
        private IValidationDictionary _validatonDictionary;
        private ITaskRepository _repository;

        public TaskService (IValidationDictionary validatonDictionary, ITaskRepository repository)
        {
            _validatonDictionary = validatonDictionary;
            _repository = repository;
        }
    }
}
