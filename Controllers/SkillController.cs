using AttestationProject.Models;

namespace AttestationProject.Controllers
{
    [Route("api/[controller]")]
    public class SkillController : ApiBaseController
    {
        #region Initialization 

        private ISkillService _service;

        public SkillController()
        {
            _service = new SkillService(new ModelStateWrapper(this.ModelState), new SkillRepository());
        }

        #endregion

        #region CRUD operations

        [HttpPost("Create Skill")]
        public async Task<IActionResult> CreateSkillAsync([FromBody]SkillInformationalModel skillEntry)
        {
            try
            {
                await _service.CreateSkillAsync(skillEntry);

                return Created("", skillEntry);
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

        [HttpGet("Get Skill by Id")]
        public async Task<IActionResult> GetSkillByIdAsync([Required] Guid Id)
        {
            try
            {
                SkillInformationalModel skill = await _service.GetSkillByIdAsync(Id);
                return Ok(skill);
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

        [HttpGet("Get Skill by Name")]
        public async Task<IActionResult> GetSkillByNameAsync([Required] SkillName skillName)
        {
            try
            {
                SkillInformationalModel skill = await _service.GetSkillByNameAsync(skillName);
                return Ok(skill);
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

        [HttpGet("Get all Skills")]
        public async Task<IActionResult> GetAllSkillsAsync()
        {
            try
            {
                List<SkillInformationalModel>  skills = await _service.GetAllSkillsAsync();
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

        [HttpPut("Update Skill")]
        public async Task<IActionResult> UpdateSkillAsync([FromBody] SkillUpdateModel skillEntry)
        {
            try
            {
                await _service.UpdateSkillAsync(skillEntry);

                return Ok("Skill successfully updated.");
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

        [HttpDelete("Delete Skill")]
        public async Task<IActionResult> DeleteSkillAsync([Required] string param)
        {
            try
            {
                await _service.DeleteSkillAsync(param);

                return Ok("Skill successfully deleted.");
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
                return StatusCode((int)statusCode, "Skill entry not found.");
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode;
                if (ex.Message == "Sequence contains no elements")
                {
                    statusCode = HttpStatusCode.NotFound;
                    return StatusCode((int)statusCode, "Skill entry not found.");
                }
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion
    }
}
