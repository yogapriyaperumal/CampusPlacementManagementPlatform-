using CampusPlacementManagementPlatform.DTO.College;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollegeController : ControllerBase
    {
        private readonly ICollegeService _collegeService;

        public CollegeController(ICollegeService collegeService)
        {
            _collegeService = collegeService;
        }

        // POST: api/college
        [HttpPost]
        public async Task<IActionResult> CreateCollege(
            CollegeCreateRequest request)
        {
            try
            {
                var result = await _collegeService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetCollegeById),
                    new { id = result.CollegeId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/college
        [HttpGet]
        public async Task<IActionResult> GetAllColleges()
        {
            var result = await _collegeService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/college/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCollegeById(int id)
        {
            var result = await _collegeService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/college/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCollege(
            int id,
            CollegeUpdateRequest request)
        {
            var result = await _collegeService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(result);
        }
    }
}