using CampusPlacementManagementPlatform.DTO.College;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface ICollegeService
    {
        Task<CollegeResponse> CreateAsync(CollegeCreateRequest request);

        Task<List<CollegeResponse>> GetAllAsync();

        Task<CollegeResponse?> GetByIdAsync(int id);

        Task<CollegeResponse?> UpdateAsync(
            int id,
            CollegeUpdateRequest request);
    }
}