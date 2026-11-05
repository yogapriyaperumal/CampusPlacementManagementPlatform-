using CampusPlacementManagementPlatform.DTO.Job;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICompanyHRRepository _companyHRRepository;
        private readonly ICollegeRepository _collegeRepository;

        public JobService(
            IJobRepository jobRepository,
            ICompanyHRRepository companyHRRepository,
            ICollegeRepository collegeRepository)
        {
            _jobRepository = jobRepository;
            _companyHRRepository = companyHRRepository;
            _collegeRepository = collegeRepository;
        }

        private async Task<CompanyHR> ValidateCompanyHRAsync(int companyHRId)
        {
            var hr = await _companyHRRepository.GetByIdAsync(companyHRId);

            if (hr == null)
                throw new InvalidOperationException(
                    "Company HR not found.");

            if (hr.Status != CompanyHRStatus.Approved)
                throw new InvalidOperationException(
                    "Company HR is not approved.");

            if (hr.Company == null ||
                hr.Company.Status != CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Company is not approved.");
            }

            return hr;
        }

        private async Task ValidateJobRequestAsync(
            decimal? salaryMin,
            decimal? salaryMax,
            decimal minimumCGPA,
            int maximumBacklogs,
            int graduationYear,
            DateTime deadline,
            List<int> targetCollegeIds)
        {
            if (salaryMin < 0 || salaryMax < 0)
                throw new InvalidOperationException(
                    "Salary cannot be negative.");

            if (salaryMin.HasValue &&
                salaryMax.HasValue &&
                salaryMin.Value > salaryMax.Value)
            {
                throw new InvalidOperationException(
                    "Minimum salary cannot exceed maximum salary.");
            }

            if (minimumCGPA < 0 || minimumCGPA > 10)
                throw new InvalidOperationException(
                    "Minimum CGPA must be between 0 and 10.");

            if (maximumBacklogs < 0)
                throw new InvalidOperationException(
                    "Maximum backlogs cannot be negative.");

            if (graduationYear < 2000 || graduationYear > 2100)
                throw new InvalidOperationException(
                    "Invalid graduation year.");

            if (deadline <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Application deadline must be in the future.");

            if (targetCollegeIds == null ||
                targetCollegeIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "Select at least one target college.");
            }

            if (targetCollegeIds.Distinct().Count() !=
                targetCollegeIds.Count)
            {
                throw new InvalidOperationException(
                    "Duplicate target college IDs are not allowed.");
            }

            foreach (var collegeId in targetCollegeIds)
            {
                var college = await _collegeRepository.GetByIdAsync(collegeId);

                if (college == null)
                    throw new InvalidOperationException(
                        $"College {collegeId} not found.");

                if (college.Status != CollegeStatus.Approved)
                    throw new InvalidOperationException(
                        $"College {collegeId} is not approved.");
            }
        }

        private static JobResponse MapToResponse(JobOpening job)
        {
            return new JobResponse
            {
                JobOpeningId = job.JobOpeningId,
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                Location = job.Location,
                EmploymentType = job.EmploymentType.ToString(),
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                MinimumCGPA = job.MinimumCGPA,
                MaximumBacklogs = job.MaximumBacklogs,
                EligibleGraduationYear = job.EligibleGraduationYear,
                EligibleDepartments = job.EligibleDepartments,
                ApplicationDeadline = job.ApplicationDeadline,
                Status = job.Status.ToString(),
                CompanyId = job.CompanyId,
                CompanyName = job.Company?.CompanyName ?? string.Empty,
                CreatedByCompanyHRId = job.CreatedByCompanyHRId,
                CreatedAt = job.CreatedAt,
                PublishedAt = job.PublishedAt,
                ClosedAt = job.ClosedAt,
                TargetCollegeIds = job.TargetColleges
                    .Select(t => t.CollegeId)
                    .ToList(),
                TargetCollegeNames = job.TargetColleges
                    .Where(t => t.College != null)
                    .Select(t => t.College.CollegeName)
                    .ToList()
            };
        }

        public async Task<JobResponse> CreateAsync(
            int companyHRId,
            JobCreateRequest request)
        {
            var hr = await ValidateCompanyHRAsync(companyHRId);

            await ValidateJobRequestAsync(
                request.SalaryMin,
                request.SalaryMax,
                request.MinimumCGPA,
                request.MaximumBacklogs,
                request.EligibleGraduationYear,
                request.ApplicationDeadline,
                request.TargetCollegeIds);

            var job = new JobOpening
            {
                JobTitle = request.JobTitle.Trim(),
                JobDescription = request.JobDescription.Trim(),
                Location = request.Location.Trim(),
                EmploymentType = request.EmploymentType,
                SalaryMin = request.SalaryMin,
                SalaryMax = request.SalaryMax,
                MinimumCGPA = request.MinimumCGPA,
                MaximumBacklogs = request.MaximumBacklogs,
                EligibleGraduationYear = request.EligibleGraduationYear,
                EligibleDepartments = request.EligibleDepartments.Trim(),
                ApplicationDeadline = request.ApplicationDeadline,
                CompanyId = hr.CompanyId,
                CreatedByCompanyHRId = companyHRId,
                Status = JobStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                TargetColleges = request.TargetCollegeIds
                    .Select(collegeId => new JobTargetCollege
                    {
                        CollegeId = collegeId
                    })
                    .ToList()
            };

            var created = await _jobRepository.CreateAsync(job);

            return MapToResponse(created);
        }

        public async Task<List<JobResponse>> GetAllAsync()
        {
            var jobs = await _jobRepository.GetAllAsync();

            return jobs.Select(MapToResponse).ToList();
        }

        public async Task<JobResponse?> GetByIdAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            return job == null ? null : MapToResponse(job);
        }

        public async Task<List<JobResponse>> GetByCompanyHRAsync(
            int companyHRId)
        {
            await ValidateCompanyHRAsync(companyHRId);

            var jobs = await _jobRepository.GetByCompanyHRAsync(companyHRId);

            return jobs.Select(MapToResponse).ToList();
        }

        public async Task<List<JobResponse>> GetByCollegeAsync(int collegeId)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                throw new KeyNotFoundException("College not found.");

            if (college.Status != CollegeStatus.Approved)
                throw new InvalidOperationException(
                    "College is not approved.");

            var jobs = await _jobRepository.GetByCollegeAsync(collegeId);

            return jobs.Select(MapToResponse).ToList();
        }

        public async Task<JobResponse> UpdateAsync(
            int id,
            int companyHRId,
            JobUpdateRequest request)
        {
            await ValidateCompanyHRAsync(companyHRId);

            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                throw new KeyNotFoundException("Job opening not found.");

            if (job.CreatedByCompanyHRId != companyHRId)
                throw new InvalidOperationException(
                    "You cannot update a job created by another HR user.");

            if (job.Status != JobStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft jobs can be updated.");

            await ValidateJobRequestAsync(
                request.SalaryMin,
                request.SalaryMax,
                request.MinimumCGPA,
                request.MaximumBacklogs,
                request.EligibleGraduationYear,
                request.ApplicationDeadline,
                request.TargetCollegeIds);

            job.JobTitle = request.JobTitle.Trim();
            job.JobDescription = request.JobDescription.Trim();
            job.Location = request.Location.Trim();
            job.EmploymentType = request.EmploymentType;
            job.SalaryMin = request.SalaryMin;
            job.SalaryMax = request.SalaryMax;
            job.MinimumCGPA = request.MinimumCGPA;
            job.MaximumBacklogs = request.MaximumBacklogs;
            job.EligibleGraduationYear = request.EligibleGraduationYear;
            job.EligibleDepartments = request.EligibleDepartments.Trim();
            job.ApplicationDeadline = request.ApplicationDeadline;

            job.TargetColleges.Clear();

            foreach (var collegeId in request.TargetCollegeIds.Distinct())
            {
                job.TargetColleges.Add(new JobTargetCollege
                {
                    JobOpeningId = job.JobOpeningId,
                    CollegeId = collegeId
                });
            }

            var updated = await _jobRepository.UpdateAsync(job);

            return MapToResponse(updated);
        }

        public async Task<JobResponse> PublishAsync(
            int id,
            int companyHRId)
        {
            await ValidateCompanyHRAsync(companyHRId);

            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                throw new KeyNotFoundException("Job opening not found.");

            if (job.CreatedByCompanyHRId != companyHRId)
                throw new InvalidOperationException(
                    "You cannot publish a job created by another HR user.");

            if (job.Status != JobStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft jobs can be published.");

            if (job.ApplicationDeadline <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Application deadline has already passed.");

            if (!job.TargetColleges.Any())
                throw new InvalidOperationException(
                    "At least one target college is required.");

            job.Status = JobStatus.Published;
            job.PublishedAt = DateTime.UtcNow;

            var updated = await _jobRepository.UpdateAsync(job);

            return MapToResponse(updated);
        }

        public async Task<JobResponse> CloseAsync(
            int id,
            int companyHRId)
        {
            await ValidateCompanyHRAsync(companyHRId);

            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                throw new KeyNotFoundException("Job opening not found.");

            if (job.CreatedByCompanyHRId != companyHRId)
                throw new InvalidOperationException(
                    "You cannot close a job created by another HR user.");

            if (job.Status != JobStatus.Published)
                throw new InvalidOperationException(
                    "Only published jobs can be closed.");

            job.Status = JobStatus.Closed;
            job.ClosedAt = DateTime.UtcNow;

            var updated = await _jobRepository.UpdateAsync(job);

            return MapToResponse(updated);
        }
    }
}