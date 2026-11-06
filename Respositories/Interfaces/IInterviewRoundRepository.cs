using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IInterviewRoundRepository
    {
        Task<InterviewRound> CreateAsync(InterviewRound round);

        Task<List<InterviewRound>> GetAllAsync();

        Task<InterviewRound?> GetByIdAsync(int id);

        Task<List<InterviewRound>> GetByDriveAsync(int driveId);

        Task<bool> RoundNumberExistsAsync(
            int driveId,
            int roundNumber,
            int? excludeRoundId = null);

        Task UpdateAsync(InterviewRound round);
    }
}