using CampusPlacementManagementPlatform.DTO.PlacementOfficer;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacementOfficerController : ControllerBase
    {
        private readonly IPlacementOfficerService _officerService;

        public PlacementOfficerController(
            IPlacementOfficerService officerService)
        {
            _officerService = officerService;
        }

        // POST: api/placementofficer
        [HttpPost]
        public async Task<IActionResult> Create(
            PlacementOfficerCreateRequest request)
        {
            try
            {
                var result = await _officerService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.PlacementOfficerId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/placementofficer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _officerService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/placementofficer/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _officerService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Placement officer not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/placementofficer/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            PlacementOfficerUpdateRequest request)
        {
            var result = await _officerService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Placement officer not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/placementofficer/1/approve
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(
            int id,
            [FromQuery] string approvedBy)
        {
            try
            {
                var result = await _officerService.ApproveAsync(
                    id, approvedBy);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Placement officer not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/placementofficer/1/reject
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result = await _officerService.RejectAsync(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Placement officer not found."
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