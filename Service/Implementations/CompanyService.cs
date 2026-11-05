using CampusPlacementManagementPlatform.DTO.Company;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        // Register a company
        public async Task<CompanyResponse> CreateAsync(
            CompanyCreateRequest request)
        {
            var company = new Company
            {
                CompanyName = request.CompanyName.Trim(),
                CompanyEmail = request.CompanyEmail.Trim(),
                Website = request.Website?.Trim(),
                Industry = request.Industry.Trim(),
                Address = request.Address.Trim(),
                Description = request.Description?.Trim(),
                Status = CompanyStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdCompany =
                await _companyRepository.CreateAsync(company);

            return MapToResponse(createdCompany);
        }

        // Retrieve all companies
        public async Task<List<CompanyResponse>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();

            return companies.Select(MapToResponse).ToList();
        }

        // Retrieve company by ID
        public async Task<CompanyResponse?> GetByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            return company == null ? null : MapToResponse(company);
        }

        // Update company information
        public async Task<CompanyResponse?> UpdateAsync(
            int id,
            CompanyUpdateRequest request)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
            {
                return null;
            }

            company.CompanyName = request.CompanyName.Trim();
            company.CompanyEmail = request.CompanyEmail.Trim();
            company.Website = request.Website?.Trim();
            company.Industry = request.Industry.Trim();
            company.Address = request.Address.Trim();
            company.Description = request.Description?.Trim();

            await _companyRepository.UpdateAsync(company);

            return MapToResponse(company);
        }

        // Convert entity into response DTO
        private static CompanyResponse MapToResponse(Company company)
        {
            return new CompanyResponse
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyEmail = company.CompanyEmail,
                Website = company.Website,
                Industry = company.Industry,
                Address = company.Address,
                Description = company.Description,
                Status = company.Status.ToString(),
                CreatedAt = company.CreatedAt
            };
        }
    }
}