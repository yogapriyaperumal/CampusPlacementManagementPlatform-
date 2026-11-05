using CampusPlacementManagementPlatform.DTO.Company;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagementPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // POST: api/company
        [HttpPost]
        public async Task<IActionResult> Create(
            CompanyCreateRequest request)
        {
            var result = await _companyService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.CompanyId },
                result);
        }

        // GET: api/company
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _companyService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/company/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _companyService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Company not found."
                });
            }

            return Ok(result);
        }

        // PUT: api/company/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CompanyUpdateRequest request)
        {
            var result = await _companyService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Company not found."
                });
            }

            return Ok(result);
        }
    }
}