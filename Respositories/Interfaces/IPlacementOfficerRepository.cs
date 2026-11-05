using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IPlacementOfficerRepository
    {
        Task<PlacementOfficer> CreateAsync(PlacementOfficer officer);

        Task<List<PlacementOfficer>> GetAllAsync();

        Task<PlacementOfficer?> GetByIdAsync(int id);

        Task<PlacementOfficer?> GetByUserIdAsync(string userId);

        Task UpdateAsync(PlacementOfficer officer);
    }
}