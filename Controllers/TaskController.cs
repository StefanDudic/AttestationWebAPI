namespace AttestationProject.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : ApiBaseController
    {
        #region Initialization 

        ITaskService _service;
     
        public TaskController()
        {
            _service = new TaskService(new ModelStateWrapper(this.ModelState), new TaskRepository(), new SkillRepository());
        }

        #endregion

        #region CRUD operations
        [HttpPost("Create Task")]
        public async Task<IActionResult> CreateSkillAsync([FromForm] TaskInformationalModel taskEntry, SkillName skillName)
        {
            try
            {
                await _service.CreateTaskAsync(taskEntry, skillName);

                return Created("", taskEntry);
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode statusCode;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = ex.StatusCode.Value;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return StatusCode((int)statusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet("Get Task by Id")]
        public async Task<IActionResult> GetTaskByIdAsync([Required] Guid skillId,[Required] Guid taskId)
        {
            try
            {
                TaskInformationalModel task = await _service.GetTaskByIdAsync(skillId, taskId);
                return Ok(task);
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode statusCode;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = ex.StatusCode.Value;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return StatusCode((int)statusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("Get All Tasks")]
        public async Task<IActionResult> GetAllSkillsAsync()
        {
            try
            {
                List<TaskInformationalModel> skills = await _service.GetAllTasksAsync();
                return Ok(skills);
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode statusCode;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = ex.StatusCode.Value;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return StatusCode((int)statusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPut("Update Task")]
        public async Task<IActionResult> UpdateTaskAsync([FromForm] TaskModel taskEntry, Guid skillId)
        {
            try
            {
                await _service.UpdateTaskAsync(taskEntry, skillId);

                return Ok("Task successfully updated.");
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode statusCode;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = ex.StatusCode.Value;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return StatusCode((int)statusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("Delete Task")]
        public async Task<IActionResult> DeleteSkillAsync([Required]Guid skillId,[Required]Guid taskId)
        {
            try
            {
                await _service.DeleteTaskAsync(skillId, taskId);

                return Ok("Task successfully deleted.");
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode statusCode;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = ex.StatusCode.Value;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
                return StatusCode((int)statusCode, "Task entry not found.");
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode;
                if (ex.Message == "Sequence contains no elements")
                {
                    statusCode = HttpStatusCode.NotFound;
                    return StatusCode((int)statusCode, "Task entry not found.");
                }
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion
    }
}
