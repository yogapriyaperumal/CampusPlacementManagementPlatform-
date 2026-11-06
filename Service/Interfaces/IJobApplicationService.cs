using CampusPlacementManagementPlatform.DTO.JobApplication;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IJobApplicationService
    {
        Task<JobApplicationResponse> ApplyAsync(
            JobApplicationCreateRequest request);

        Task<List<JobApplicationResponse>> GetAllAsync();

        Task<JobApplicationResponse?> GetByIdAsync(int id);

        Task<List<JobApplicationResponse>> GetByStudentAsync(
            int studentId);

        Task<List<JobApplicationResponse>> GetByJobAsync(
            int jobOpeningId);

        Task<JobApplicationResponse> UpdateStatusAsync(
            int applicationId,
            int companyHRId,
            string updatedByUserId,
            ApplicationStatusUpdateRequest request);

        Task<JobApplicationResponse> WithdrawAsync(
            int applicationId,
            int studentId);

        Task<List<ApplicationStatusHistoryResponse>> GetHistoryAsync(
            int applicationId);
    }
}