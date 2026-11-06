using CampusPlacementManagementPlatform.DTO.InterviewRound;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [ApiController]
    [Route("api")]
    public class InterviewRoundController : ControllerBase
    {
        private readonly IInterviewRoundService _service;

        public InterviewRoundController(IInterviewRoundService service)
        {
            _service = service;
        }

        [HttpPost("recruitmentdrives/{driveId:int}/rounds")]
        public async Task<IActionResult> Create(
            int driveId,
            InterviewRoundCreateRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(driveId, request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.InterviewRoundId },
                    result);
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

        [HttpGet("interviewrounds")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("interviewrounds/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("recruitmentdrives/{driveId:int}/rounds")]
        public async Task<IActionResult> GetByDrive(int driveId)
        {
            try
            {
                return Ok(await _service.GetByDriveAsync(driveId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("interviewrounds/{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            InterviewRoundUpdateRequest request)
        {
            try
            {
                return Ok(await _service.UpdateAsync(id, request));
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

        [HttpPut("interviewrounds/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            InterviewRoundStatusUpdateRequest request)
        {
            try
            {
                return Ok(await _service.UpdateStatusAsync(id, request));
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