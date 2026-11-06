using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication> CreateAsync(JobApplication application);

        Task<JobApplication?> GetByIdAsync(int id);

        Task<JobApplication?> GetByStudentAndJobAsync(
            int studentId,
            int jobOpeningId);

        Task<List<JobApplication>> GetAllAsync();

        Task<List<JobApplication>> GetByStudentAsync(int studentId);

        Task<List<JobApplication>> GetByJobAsync(int jobOpeningId);

        Task<JobApplication> UpdateAsync(JobApplication application);

        Task<List<ApplicationStatusHistory>> GetHistoryAsync(
            int jobApplicationId);

        Task AddHistoryAsync(ApplicationStatusHistory history);
    }
}