using CampusPlacementManagementPlatform.DTO.Job;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IJobService
    {
        Task<JobResponse> CreateAsync(int companyHRId,
            JobCreateRequest request);

        Task<List<JobResponse>> GetAllAsync();

        Task<JobResponse?> GetByIdAsync(int id);

        Task<List<JobResponse>> GetByCompanyHRAsync(int companyHRId);

        Task<List<JobResponse>> GetByCollegeAsync(int collegeId);

        Task<JobResponse> UpdateAsync(
            int id,
            int companyHRId,
            JobUpdateRequest request);

        Task<JobResponse> PublishAsync(int id, int companyHRId);

        Task<JobResponse> CloseAsync(int id, int companyHRId);
    }
}