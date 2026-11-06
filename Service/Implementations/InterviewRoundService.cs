using CampusPlacementManagementPlatform.DTO.InterviewRound;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Services.Implementations
{
    public class InterviewRoundService : IInterviewRoundService
    {
        private readonly IInterviewRoundRepository _roundRepository;
        private readonly IRecruitmentDriveRepository _driveRepository;

        public InterviewRoundService(
            IInterviewRoundRepository roundRepository,
            IRecruitmentDriveRepository driveRepository)
        {
            _roundRepository = roundRepository;
            _driveRepository = driveRepository;
        }

        public async Task<InterviewRoundResponse> CreateAsync(
            int driveId,
            InterviewRoundCreateRequest request)
        {
            var drive = await _driveRepository.GetByIdAsync(driveId)
                ?? throw new KeyNotFoundException(
                    "Recruitment drive not found.");

            if (drive.Status != RecruitmentDriveStatus.Open &&
                drive.Status != RecruitmentDriveStatus.InProgress)
            {
                throw new InvalidOperationException(
                    "Interview rounds can only be added to open or in-progress drives.");
            }

            if (!Enum.IsDefined(typeof(InterviewRoundType), request.RoundType))
            {
                throw new InvalidOperationException(
                    "Invalid interview round type.");
            }

            if (await _roundRepository.RoundNumberExistsAsync(
                driveId, request.RoundNumber))
            {
                throw new InvalidOperationException(
                    "This round number already exists for the recruitment drive.");
            }

            var round = new InterviewRound
            {
                RecruitmentDriveId = driveId,
                RoundName = request.RoundName.Trim(),
                RoundNumber = request.RoundNumber,
                RoundType = request.RoundType,
                Description = request.Description?.Trim(),
                DurationMinutes = request.DurationMinutes,
                Status = InterviewRoundStatus.Planned,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _roundRepository.CreateAsync(round);

            return MapToResponse(created);
        }

        public async Task<List<InterviewRoundResponse>> GetAllAsync()
        {
            var rounds = await _roundRepository.GetAllAsync();

            return rounds.Select(MapToResponse).ToList();
        }

        public async Task<InterviewRoundResponse> GetByIdAsync(int id)
        {
            var round = await _roundRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Interview round not found.");

            return MapToResponse(round);
        }

        public async Task<List<InterviewRoundResponse>> GetByDriveAsync(
            int driveId)
        {
            var drive = await _driveRepository.GetByIdAsync(driveId)
                ?? throw new KeyNotFoundException(
                    "Recruitment drive not found.");

            var rounds = await _roundRepository.GetByDriveAsync(driveId);

            return rounds.Select(MapToResponse).ToList();
        }

        public async Task<InterviewRoundResponse> UpdateAsync(
            int id,
            InterviewRoundUpdateRequest request)
        {
            var round = await _roundRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Interview round not found.");

            if (round.Status != InterviewRoundStatus.Planned)
            {
                throw new InvalidOperationException(
                    "Only planned interview rounds can be edited.");
            }

            if (!Enum.IsDefined(typeof(InterviewRoundType), request.RoundType))
            {
                throw new InvalidOperationException(
                    "Invalid interview round type.");
            }

            if (await _roundRepository.RoundNumberExistsAsync(
                round.RecruitmentDriveId,
                request.RoundNumber,
                round.InterviewRoundId))
            {
                throw new InvalidOperationException(
                    "This round number already exists for the recruitment drive.");
            }

            round.RoundName = request.RoundName.Trim();
            round.RoundNumber = request.RoundNumber;
            round.RoundType = request.RoundType;
            round.Description = request.Description?.Trim();
            round.DurationMinutes = request.DurationMinutes;

            await _roundRepository.UpdateAsync(round);

            var updated = await _roundRepository.GetByIdAsync(id);

            return MapToResponse(updated!);
        }

        public async Task<InterviewRoundResponse> UpdateStatusAsync(
            int id,
            InterviewRoundStatusUpdateRequest request)
        {
            var round = await _roundRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Interview round not found.");

            bool validTransition = round.Status switch
            {
                InterviewRoundStatus.Planned =>
                    request.Status == InterviewRoundStatus.Active ||
                    request.Status == InterviewRoundStatus.Cancelled,

                InterviewRoundStatus.Active =>
                    request.Status == InterviewRoundStatus.Completed ||
                    request.Status == InterviewRoundStatus.Cancelled,

                _ => false
            };

            if (!validTransition)
            {
                throw new InvalidOperationException(
                    $"Cannot change interview round status from {round.Status} to {request.Status}.");
            }

            round.Status = request.Status;

            await _roundRepository.UpdateAsync(round);

            var updated = await _roundRepository.GetByIdAsync(id);

            return MapToResponse(updated!);
        }

        private static InterviewRoundResponse MapToResponse(
            InterviewRound round)
        {
            return new InterviewRoundResponse
            {
                InterviewRoundId = round.InterviewRoundId,
                RecruitmentDriveId = round.RecruitmentDriveId,
                DriveTitle = round.RecruitmentDrive.DriveTitle,
                RoundName = round.RoundName,
                RoundNumber = round.RoundNumber,
                RoundType = round.RoundType.ToString(),
                Description = round.Description,
                DurationMinutes = round.DurationMinutes,
                Status = round.Status.ToString(),
                CreatedAt = round.CreatedAt
            };
        }
    }
}