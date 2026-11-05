using CampusPlacementManagementPlatform.DTO.Job;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("create/{companyHRId}")]
        public async Task<IActionResult> Create(
            int companyHRId,
            JobCreateRequest request)
        {
            try
            {
                var job = await _jobService.CreateAsync(
                    companyHRId, request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = job.JobOpeningId },
                    job);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobService.GetAllAsync();

            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobService.GetByIdAsync(id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            return Ok(job);
        }

        [HttpGet("companyhr/{companyHRId}")]
        public async Task<IActionResult> GetByCompanyHR(int companyHRId)
        {
            try
            {
                var jobs =
                    await _jobService.GetByCompanyHRAsync(companyHRId);

                return Ok(jobs);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("college/{collegeId}")]
        public async Task<IActionResult> GetByCollege(int collegeId)
        {
            try
            {
                var jobs = await _jobService.GetByCollegeAsync(collegeId);

                return Ok(jobs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/companyhr/{companyHRId}")]
        public async Task<IActionResult> Update(
            int id,
            int companyHRId,
            JobUpdateRequest request)
        {
            try
            {
                var job = await _jobService.UpdateAsync(
                    id, companyHRId, request);

                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/publish/{companyHRId}")]
        public async Task<IActionResult> Publish(
            int id,
            int companyHRId)
        {
            try
            {
                var job = await _jobService.PublishAsync(id, companyHRId);

                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/close/{companyHRId}")]
        public async Task<IActionResult> Close(
            int id,
            int companyHRId)
        {
            try
            {
                var job = await _jobService.CloseAsync(id, companyHRId);

                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}