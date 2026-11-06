using CampusPlacementManagementPlatform.DTO.JobApplication;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ICompanyHRRepository _companyHRRepository;

        public JobApplicationService(
            IJobApplicationRepository applicationRepository,
            IStudentRepository studentRepository,
            IJobRepository jobRepository,
            ICompanyHRRepository companyHRRepository)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _jobRepository = jobRepository;
            _companyHRRepository = companyHRRepository;
        }

        private static JobApplicationResponse MapToResponse(
            JobApplication application)
        {
            return new JobApplicationResponse
            {
                JobApplicationId = application.JobApplicationId,
                StudentId = application.StudentId,
                RegisterNumber =
                    application.Student?.RegisterNumber ?? string.Empty,
                Department =
                    application.Student?.Department ?? string.Empty,
                JobOpeningId = application.JobOpeningId,
                JobTitle =
                    application.JobOpening?.JobTitle ?? string.Empty,
                CompanyName =
                    application.JobOpening?.Company?.CompanyName
                    ?? string.Empty,
                CoverLetter = application.CoverLetter,
                Status = application.Status.ToString(),
                AppliedAt = application.AppliedAt,
                LastUpdatedAt = application.LastUpdatedAt
            };
        }

        private static bool IsDepartmentEligible(
            string studentDepartment,
            string eligibleDepartments)
        {
            var departments = eligibleDepartments
                .Split(',', StringSplitOptions.RemoveEmptyEntries |
                           StringSplitOptions.TrimEntries);

            return departments.Any(d =>
                d.Equals(
                    studentDepartment.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        private static void ValidateEligibility(
            Student student,
            JobOpening job)
        {
            if (student.Status != StudentStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved students can apply.");

            if (job.Status != JobStatus.Published)
                throw new InvalidOperationException(
                    "Applications are accepted only for published jobs.");

            if (job.ApplicationDeadline <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    "The application deadline has passed.");

            if (!job.TargetColleges.Any(t =>
                t.CollegeId == student.CollegeId))
            {
                throw new InvalidOperationException(
                    "This job is not available for your college.");
            }

            if (student.CGPA < job.MinimumCGPA)
                throw new InvalidOperationException(
                    "Your CGPA does not meet the job requirement.");

            if (student.ActiveBacklogs > job.MaximumBacklogs)
                throw new InvalidOperationException(
                    "Your active backlogs exceed the allowed limit.");

            if (student.GraduationYear != job.EligibleGraduationYear)
                throw new InvalidOperationException(
                    "Your graduation year does not match the job requirement.");

            if (!IsDepartmentEligible(
                student.Department,
                job.EligibleDepartments))
            {
                throw new InvalidOperationException(
                    "Your department is not eligible for this job.");
            }

            if (student.College == null ||
                student.College.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Your college is not approved.");
            }
        }

        public async Task<JobApplicationResponse> ApplyAsync(
            JobApplicationCreateRequest request)
        {
            var student = await _studentRepository.GetByIdAsync(
                request.StudentId);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            var job = await _jobRepository.GetByIdAsync(
                request.JobOpeningId);

            if (job == null)
                throw new KeyNotFoundException("Job opening not found.");

            ValidateEligibility(student, job);

            var existing = await _applicationRepository
                .GetByStudentAndJobAsync(
                    request.StudentId,
                    request.JobOpeningId);

            if (existing != null)
                throw new InvalidOperationException(
                    "You have already applied for this job.");

            var application = new JobApplication
            {
                StudentId = student.StudentId,
                JobOpeningId = job.JobOpeningId,
                CoverLetter = request.CoverLetter?.Trim(),
                Status = ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow
            };

            // Record the initial application status.
            application.StatusHistory.Add(
                new ApplicationStatusHistory
                {
                    PreviousStatus = ApplicationStatus.Applied,
                    NewStatus = ApplicationStatus.Applied,
                    Remarks = "Application submitted.",
                    ChangedAt = DateTime.UtcNow
                });

            // Application and initial history are saved together.
            var created = await _applicationRepository.CreateAsync(
                application);

            return MapToResponse(created);
        }

        public async Task<List<JobApplicationResponse>> GetAllAsync()
        {
            var applications =
                await _applicationRepository.GetAllAsync();

            return applications.Select(MapToResponse).ToList();
        }

        public async Task<JobApplicationResponse?> GetByIdAsync(int id)
        {
            var application =
                await _applicationRepository.GetByIdAsync(id);

            return application == null
                ? null
                : MapToResponse(application);
        }

        public async Task<List<JobApplicationResponse>> GetByStudentAsync(
            int studentId)
        {
            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            var applications =
                await _applicationRepository.GetByStudentAsync(studentId);

            return applications.Select(MapToResponse).ToList();
        }

        public async Task<List<JobApplicationResponse>> GetByJobAsync(
            int jobOpeningId)
        {
            var job = await _jobRepository.GetByIdAsync(jobOpeningId);

            if (job == null)
                throw new KeyNotFoundException("Job opening not found.");

            var applications =
                await _applicationRepository.GetByJobAsync(jobOpeningId);

            return applications.Select(MapToResponse).ToList();
        }

        public async Task<JobApplicationResponse> UpdateStatusAsync(
            int applicationId,
            int companyHRId,
            string updatedByUserId,
            ApplicationStatusUpdateRequest request)
        {
            var hr = await _companyHRRepository.GetByIdAsync(companyHRId);

            if (hr == null)
                throw new InvalidOperationException(
                    "Company HR not found.");

            if (hr.Status != CompanyHRStatus.Approved ||
                hr.Company == null ||
                hr.Company.Status != CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Company HR or company is not approved.");
            }

            var application =
                await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
                throw new KeyNotFoundException(
                    "Job application not found.");

            if (application.JobOpening.CreatedByCompanyHRId != companyHRId)
                throw new InvalidOperationException(
                    "You cannot manage applications for another HR user's job.");

            var currentStatus = application.Status;
            var newStatus = request.NewStatus;

            bool validTransition = currentStatus switch
            {
                ApplicationStatus.Applied =>
                    newStatus == ApplicationStatus.UnderReview ||
                    newStatus == ApplicationStatus.Rejected,

                ApplicationStatus.UnderReview =>
                    newStatus == ApplicationStatus.Shortlisted ||
                    newStatus == ApplicationStatus.Rejected,

                ApplicationStatus.Shortlisted =>
                    newStatus == ApplicationStatus.Selected ||
                    newStatus == ApplicationStatus.Rejected,

                _ => false
            };

            if (!validTransition)
                throw new InvalidOperationException(
                    $"Cannot change application status from " +
                    $"{currentStatus} to {newStatus}.");

            application.Status = newStatus;
            application.LastUpdatedAt = DateTime.UtcNow;

            application.StatusHistory.Add(
                new ApplicationStatusHistory
                {
                    PreviousStatus = currentStatus,
                    NewStatus = newStatus,
                    Remarks = request.Remarks?.Trim(),
                    UpdatedByUserId = updatedByUserId,
                    ChangedAt = DateTime.UtcNow
                });

            var updated = await _applicationRepository.UpdateAsync(
                application);

            return MapToResponse(updated);
        }

        public async Task<JobApplicationResponse> WithdrawAsync(
            int applicationId,
            int studentId)
        {
            var application =
                await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
                throw new KeyNotFoundException(
                    "Job application not found.");

            if (application.StudentId != studentId)
                throw new InvalidOperationException(
                    "You cannot withdraw another student's application.");

            if (application.Status != ApplicationStatus.Applied &&
                application.Status != ApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException(
                    "This application can no longer be withdrawn.");
            }

            var previousStatus = application.Status;

            application.Status = ApplicationStatus.Withdrawn;
            application.LastUpdatedAt = DateTime.UtcNow;

            application.StatusHistory.Add(
                new ApplicationStatusHistory
                {
                    PreviousStatus = previousStatus,
                    NewStatus = ApplicationStatus.Withdrawn,
                    Remarks = "Application withdrawn by student.",
                    ChangedAt = DateTime.UtcNow
                });

            var updated = await _applicationRepository.UpdateAsync(
                application);

            return MapToResponse(updated);
        }

        public async Task<List<ApplicationStatusHistoryResponse>>
            GetHistoryAsync(int applicationId)
        {
            var application =
                await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
                throw new KeyNotFoundException(
                    "Job application not found.");

            var history = await _applicationRepository.GetHistoryAsync(
                applicationId);

            return history.Select(h => new ApplicationStatusHistoryResponse
            {
                ApplicationStatusHistoryId =
                    h.ApplicationStatusHistoryId,
                PreviousStatus = h.PreviousStatus.ToString(),
                NewStatus = h.NewStatus.ToString(),
                Remarks = h.Remarks,
                UpdatedByUserId = h.UpdatedByUserId,
                ChangedAt = h.ChangedAt
            }).ToList();
        }
    }
}