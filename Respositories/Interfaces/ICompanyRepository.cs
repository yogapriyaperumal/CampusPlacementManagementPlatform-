using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> CreateAsync(Company company);

        Task<List<Company>> GetAllAsync();

        Task<Company?> GetByIdAsync(int id);

        Task UpdateAsync(Company company);
    }
}