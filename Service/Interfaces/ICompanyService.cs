using CampusPlacementManagementPlatform.DTO.Company;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyResponse> CreateAsync(CompanyCreateRequest request);

        Task<List<CompanyResponse>> GetAllAsync();

        Task<CompanyResponse?> GetByIdAsync(int id);

        Task<CompanyResponse?> UpdateAsync(
            int id,
            CompanyUpdateRequest request);
    }
}