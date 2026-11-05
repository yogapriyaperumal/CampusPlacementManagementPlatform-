using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IJobRepository
    {
        Task<JobOpening> CreateAsync(JobOpening job);

        Task<List<JobOpening>> GetAllAsync();

        Task<JobOpening?> GetByIdAsync(int id);

        Task<List<JobOpening>> GetByCompanyHRAsync(int companyHRId);

        Task<List<JobOpening>> GetByCollegeAsync(int collegeId);

        Task<JobOpening> UpdateAsync(JobOpening job);
    }
}