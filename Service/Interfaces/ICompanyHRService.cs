using CampusPlacementManagementPlatform.DTO.CompanyHR;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface ICompanyHRService
    {
        Task<CompanyHRResponse> CreateAsync(
            CompanyHRCreateRequest request);

        Task<List<CompanyHRResponse>> GetAllAsync();

        Task<CompanyHRResponse?> GetByIdAsync(int id);

        Task<CompanyHRResponse?> UpdateAsync(
            int id,
            CompanyHRUpdateRequest request);

        Task<CompanyHRResponse?> ApproveAsync(
            int id,
            string approvedBy);

        Task<CompanyHRResponse?> RejectAsync(int id);
    }
}