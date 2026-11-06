using CampusPlacementManagementPlatform.DTO.RecruitmentDrive;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [ApiController]
    [Route("api/recruitmentdrives")]
    public class RecruitmentDriveController : ControllerBase
    {
        private readonly IRecruitmentDriveService _service;

        public RecruitmentDriveController(IRecruitmentDriveService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            RecruitmentDriveCreateRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.RecruitmentDriveId },
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
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

        [HttpGet("company/{companyId:int}")]
        public async Task<IActionResult> GetByCompany(int companyId)
        {
            return Ok(await _service.GetByCompanyAsync(companyId));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            RecruitmentDriveUpdateRequest request)
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

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            RecruitmentDriveStatusUpdateRequest request)
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