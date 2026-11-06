using CampusPlacementManagementPlatform.DTO.RecruitmentDrive;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class RecruitmentDriveService : IRecruitmentDriveService
    {
        private readonly IRecruitmentDriveRepository _driveRepository;
        private readonly ICompanyHRRepository _companyHRRepository;
        private readonly IJobRepository _jobRepository;

        public RecruitmentDriveService(
            IRecruitmentDriveRepository driveRepository,
            ICompanyHRRepository companyHRRepository,
            IJobRepository jobRepository)
        {
            _driveRepository = driveRepository;
            _companyHRRepository = companyHRRepository;
            _jobRepository = jobRepository;
        }

        public async Task<RecruitmentDriveResponse> CreateAsync(
            RecruitmentDriveCreateRequest request)
        {
            if (request.StartDate >= request.EndDate)
                throw new InvalidOperationException(
                    "End date must be after start date.");

            if (request.StartDate < DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Drive start date cannot be in the past.");

            if (request.Mode != "Online" && request.Mode != "Offline")
                throw new InvalidOperationException(
                    "Mode must be Online or Offline.");

            var hr = await _companyHRRepository.GetByIdAsync(request.CompanyHRId)
                ?? throw new KeyNotFoundException("Company HR not found.");

            if (hr.Status != CompanyHRStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved Company HRs can create recruitment drives.");

            if (hr.Company.Status != CompanyStatus.Approved)
                throw new InvalidOperationException(
                    "Company is not approved.");

            var job = await _jobRepository.GetByIdAsync(request.JobOpeningId)
                ?? throw new KeyNotFoundException("Job opening not found.");

            if (job.CompanyId != hr.CompanyId)
                throw new InvalidOperationException(
                    "Company HR can only create drives for their own company.");

            if (job.Status != JobStatus.Published)
                throw new InvalidOperationException(
                    "A recruitment drive can only be created for a published job.");

            if (request.Mode == "Online" &&
                string.IsNullOrWhiteSpace(request.VenueOrMeetingLink))
                throw new InvalidOperationException(
                    "Meeting link is required for online drives.");

            if (request.Mode == "Offline" &&
                string.IsNullOrWhiteSpace(request.VenueOrMeetingLink))
                throw new InvalidOperationException(
                    "Venue is required for offline drives.");

            var drive = new RecruitmentDrive
            {
                DriveTitle = request.DriveTitle.Trim(),
                Description = request.Description?.Trim(),
                CompanyId = hr.CompanyId,
                CompanyHRId = hr.CompanyHRId,
                JobOpeningId = job.JobOpeningId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Mode = request.Mode,
                VenueOrMeetingLink = request.VenueOrMeetingLink?.Trim(),
                Status = RecruitmentDriveStatus.Planned,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _driveRepository.CreateAsync(drive);

            return MapToResponse(created);
        }

        public async Task<List<RecruitmentDriveResponse>> GetAllAsync()
        {
            var drives = await _driveRepository.GetAllAsync();

            return drives.Select(MapToResponse).ToList();
        }

        public async Task<RecruitmentDriveResponse> GetByIdAsync(int id)
        {
            var drive = await _driveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Recruitment drive not found.");

            return MapToResponse(drive);
        }

        public async Task<List<RecruitmentDriveResponse>> GetByCompanyAsync(
            int companyId)
        {
            var drives = await _driveRepository.GetByCompanyAsync(companyId);

            return drives.Select(MapToResponse).ToList();
        }

        public async Task<RecruitmentDriveResponse> UpdateAsync(
            int id,
            RecruitmentDriveUpdateRequest request)
        {
            var drive = await _driveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Recruitment drive not found.");

            if (drive.Status == RecruitmentDriveStatus.Completed ||
                drive.Status == RecruitmentDriveStatus.Cancelled)
                throw new InvalidOperationException(
                    "Completed or cancelled drives cannot be edited.");

            if (request.StartDate >= request.EndDate)
                throw new InvalidOperationException(
                    "End date must be after start date.");

            if (request.Mode != "Online" && request.Mode != "Offline")
                throw new InvalidOperationException(
                    "Mode must be Online or Offline.");

            if (string.IsNullOrWhiteSpace(request.VenueOrMeetingLink))
                throw new InvalidOperationException(
                    "Venue or meeting link is required.");

            drive.DriveTitle = request.DriveTitle.Trim();
            drive.Description = request.Description?.Trim();
            drive.StartDate = request.StartDate;
            drive.EndDate = request.EndDate;
            drive.Mode = request.Mode;
            drive.VenueOrMeetingLink = request.VenueOrMeetingLink.Trim();

            await _driveRepository.UpdateAsync(drive);

            var updated = await _driveRepository.GetByIdAsync(id);

            return MapToResponse(updated!);
        }

        public async Task<RecruitmentDriveResponse> UpdateStatusAsync(
            int id,
            RecruitmentDriveStatusUpdateRequest request)
        {
            var drive = await _driveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Recruitment drive not found.");

            bool validTransition = drive.Status switch
            {
                RecruitmentDriveStatus.Planned =>
                    request.Status == RecruitmentDriveStatus.Open ||
                    request.Status == RecruitmentDriveStatus.Cancelled,

                RecruitmentDriveStatus.Open =>
                    request.Status == RecruitmentDriveStatus.InProgress ||
                    request.Status == RecruitmentDriveStatus.Cancelled,

                RecruitmentDriveStatus.InProgress =>
                    request.Status == RecruitmentDriveStatus.Completed ||
                    request.Status == RecruitmentDriveStatus.Cancelled,

                _ => false
            };

            if (!validTransition)
                throw new InvalidOperationException(
                    $"Cannot change drive status from {drive.Status} to {request.Status}.");

            drive.Status = request.Status;

            await _driveRepository.UpdateAsync(drive);

            var updated = await _driveRepository.GetByIdAsync(id);

            return MapToResponse(updated!);
        }

        private static RecruitmentDriveResponse MapToResponse(
            RecruitmentDrive drive)
        {
            return new RecruitmentDriveResponse
            {
                RecruitmentDriveId = drive.RecruitmentDriveId,
                DriveTitle = drive.DriveTitle,
                Description = drive.Description,
                CompanyId = drive.CompanyId,
                CompanyName = drive.Company.CompanyName,
                CompanyHRId = drive.CompanyHRId,
                JobOpeningId = drive.JobOpeningId,
                JobTitle = drive.JobOpening.JobTitle,
                StartDate = drive.StartDate,
                EndDate = drive.EndDate,
                Mode = drive.Mode,
                VenueOrMeetingLink = drive.VenueOrMeetingLink,
                Status = drive.Status.ToString(),
                CreatedAt = drive.CreatedAt
            };
        }
    }
}