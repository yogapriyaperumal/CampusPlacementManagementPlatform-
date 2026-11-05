using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface ICompanyHRRepository
    {
        Task<CompanyHR> CreateAsync(CompanyHR companyHR);

        Task<List<CompanyHR>> GetAllAsync();

        Task<CompanyHR?> GetByIdAsync(int id);

        Task<CompanyHR?> GetByUserIdAsync(string userId);

        Task UpdateAsync(CompanyHR companyHR);
    }
}