namespace AttestationProject.Controllers
{
    [Route("api/[controller]")]
    public class CandidateController : ApiBaseController
    {
        #region Initialization 

        private ICandidateService _service;

        public CandidateController()
        {
            _service = new CandidateService(new ModelStateWrapper(this.ModelState), new CandidateRepository());
        }

        #endregion

        #region CRUD operations

        [HttpPost("Create Candidate")]
        public async Task<IActionResult> CreateCandidateAsync([FromForm] CandidateInformationalModel candidateEntry)
        {
            try
            {
                await _service.CreateCandidateAsync(candidateEntry);

                return Created("", candidateEntry);
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

        [HttpGet("Get Candidate by Id")]
        public async Task<IActionResult> GetCandidateByIdAsync([Required] Guid Id)
        {
            try
            {
                CandidateInformationalModel candidate = await _service.GetCandidateByIdAsync(Id);
                return Ok(candidate);
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

        [HttpGet("Get Candidates by Senioriy")]
        public async Task<IActionResult> GetCandidateBySeniorityAsync([Required] Seniority seniority)
        {
            try
            {
                List<CandidateInformationalModel> candidates = await _service.GetCandidatesBySeniorityAsync(seniority);
                return Ok(candidates);
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

        [HttpGet("Get all Candidates")]
        public async Task<IActionResult> GetAllCandidatesAsync()
        {
            try
            {
                List<CandidateInformationalModel> candidates = await _service.GetAllCandidatesAsync();
                return Ok(candidates);
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

        [HttpPut("Update Candidate")]
        public async Task<IActionResult> UpdateCandidateAsync([FromForm][Required] CandidateModel candidateEntry)
        {
            try
            {
                await _service.UpdateCandidateAsync(candidateEntry);

                return Ok("Candidate successfully updated.");
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

        [HttpDelete("Delete Candidate")]
        public async Task<IActionResult> DeleteCandidateAsync([Required] Guid id)
        {
            try
            {
                await _service.DeleteCandidateAsync(id);

                return Ok("Candidate successfully deleted.");
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
                return StatusCode((int)statusCode, "Table entry not found.");
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
