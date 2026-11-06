using CampusPlacementManagementPlatform.DTO.JobApplication;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/jobapplications")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public JobApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        // Apply for a job.
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
            JobApplicationCreateRequest request)
        {
            try
            {
                var application = await _service.ApplyAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = application.JobApplicationId },
                    application);
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

        // Get all applications.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _service.GetAllAsync();

            return Ok(applications);
        }

        // Get application by ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = await _service.GetByIdAsync(id);

            if (application == null)
                return NotFound(new { message = "Application not found." });

            return Ok(application);
        }

        // Get applications submitted by a student.
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            try
            {
                var applications =
                    await _service.GetByStudentAsync(studentId);

                return Ok(applications);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Get applications received for a job.
        [HttpGet("job/{jobOpeningId}")]
        public async Task<IActionResult> GetByJob(int jobOpeningId)
        {
            try
            {
                var applications =
                    await _service.GetByJobAsync(jobOpeningId);

                return Ok(applications);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Update application status by the responsible CompanyHR.
        [HttpPut("{id}/status/companyhr/{companyHRId}")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            int companyHRId,
            [FromQuery] string updatedByUserId,
            ApplicationStatusUpdateRequest request)
        {
            try
            {
                var application = await _service.UpdateStatusAsync(
                    id,
                    companyHRId,
                    updatedByUserId,
                    request);

                return Ok(application);
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

        // Withdraw application by student.
        [HttpPut("{id}/withdraw/{studentId}")]
        public async Task<IActionResult> Withdraw(
            int id,
            int studentId)
        {
            try
            {
                var application = await _service.WithdrawAsync(
                    id, studentId);

                return Ok(application);
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

        // Retrieve application status history.
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id)
        {
            try
            {
                var history = await _service.GetHistoryAsync(id);

                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}