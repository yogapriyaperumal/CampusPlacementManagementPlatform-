using CampusPlacementManagementPlatform.DTO.InterviewRound;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IInterviewRoundService
    {
        Task<InterviewRoundResponse> CreateAsync(
            int driveId,
            InterviewRoundCreateRequest request);

        Task<List<InterviewRoundResponse>> GetAllAsync();

        Task<InterviewRoundResponse> GetByIdAsync(int id);

        Task<List<InterviewRoundResponse>> GetByDriveAsync(int driveId);

        Task<InterviewRoundResponse> UpdateAsync(
            int id,
            InterviewRoundUpdateRequest request);

        Task<InterviewRoundResponse> UpdateStatusAsync(
            int id,
            InterviewRoundStatusUpdateRequest request);
    }
}