using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface ICollegeRepository
    {
        Task<College> CreateAsync(College college);

        Task<List<College>> GetAllAsync();

        Task<College?> GetByIdAsync(int id);

        Task<College?> GetByCodeAsync(string collegeCode);

        Task UpdateAsync(College college);
    }
}