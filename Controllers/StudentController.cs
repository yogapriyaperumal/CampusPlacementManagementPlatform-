using CampusPlacementManagementPlatform.DTO.Student;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Enroll a student under a placement officer.
        [HttpPost("enroll/{officerId}")]
        public async Task<IActionResult> Create(
            int officerId,
            StudentCreateRequest request)
        {
            try
            {
                var student = await _studentService.CreateAsync(
                    officerId, request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = student.StudentId },
                    student);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Get all students.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();

            return Ok(students);
        }

        // Get a student by ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);

            if (student == null)
                return NotFound(new { message = "Student not found." });

            return Ok(student);
        }

        // Get students managed by a particular officer.
        [HttpGet("officer/{officerId}")]
        public async Task<IActionResult> GetByOfficer(int officerId)
        {
            try
            {
                var students =
                    await _studentService.GetByOfficerIdAsync(officerId);

                return Ok(students);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Update student academic details.
        [HttpPut("{id}/officer/{officerId}")]
        public async Task<IActionResult> Update(
            int id,
            int officerId,
            StudentUpdateRequest request)
        {
            try
            {
                var student = await _studentService.UpdateAsync(
                    id, officerId, request);

                return Ok(student);
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

        // Approve a pending student.
        [HttpPut("{id}/approve/{officerId}")]
        public async Task<IActionResult> Approve(
            int id,
            int officerId)
        {
            try
            {
                var student = await _studentService.ApproveAsync(
                    id, officerId);

                return Ok(student);
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

        // Reject a pending student.
        [HttpPut("{id}/reject/{officerId}")]
        public async Task<IActionResult> Reject(
            int id,
            int officerId)
        {
            try
            {
                var student = await _studentService.RejectAsync(
                    id, officerId);

                return Ok(student);
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

        // Bulk import students using Excel.
        [HttpPost("bulk-import/{officerId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportExcel(
            int officerId,
            IFormFile file)
        {
            try
            {
                var result = await _studentService.ImportExcelAsync(
                    officerId, file);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}