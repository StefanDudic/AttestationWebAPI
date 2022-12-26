namespace AttestationProject.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : ApiBaseController
    {
        #region Initialization 

        ITaskService _service;

        public TaskController()
        {
            _service = new TaskService(new ModelStateWrapper(this.ModelState), new TaskRepository());
        }

        #endregion
    }
}
