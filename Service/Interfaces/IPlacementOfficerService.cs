using CampusPlacementManagementPlatform.DTO.PlacementOfficer;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IPlacementOfficerService
    {
        Task<PlacementOfficerResponse> CreateAsync(
            PlacementOfficerCreateRequest request);

        Task<List<PlacementOfficerResponse>> GetAllAsync();

        Task<PlacementOfficerResponse?> GetByIdAsync(int id);

        Task<PlacementOfficerResponse?> UpdateAsync(
            int id,
            PlacementOfficerUpdateRequest request);

        Task<PlacementOfficerResponse?> ApproveAsync(
            int id,
            string approvedBy);

        Task<PlacementOfficerResponse?> RejectAsync(int id);
    }
}