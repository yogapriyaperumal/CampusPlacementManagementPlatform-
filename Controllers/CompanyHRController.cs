using CampusPlacementManagementPlatform.DTO.CompanyHR;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyHRController : ControllerBase
    {
        private readonly ICompanyHRService _companyHRService;

        public CompanyHRController(ICompanyHRService companyHRService)
        {
            _companyHRService = companyHRService;
        }

        // POST: api/companyhr
        [HttpPost]
        public async Task<IActionResult> Create(
            CompanyHRCreateRequest request)
        {
            try
            {
                var result =
                    await _companyHRService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.CompanyHRId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/companyhr
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _companyHRService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/companyhr/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _companyHRService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "CompanyHR not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/companyhr/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CompanyHRUpdateRequest request)
        {
            var result =
                await _companyHRService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "CompanyHR not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/companyhr/1/approve?approvedBy=USER_ID
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(
            int id,
            [FromQuery] string approvedBy)
        {
            try
            {
                var result =
                    await _companyHRService.ApproveAsync(id, approvedBy);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "CompanyHR not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/companyhr/1/reject
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result =
                    await _companyHRService.RejectAsync(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "CompanyHR not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}