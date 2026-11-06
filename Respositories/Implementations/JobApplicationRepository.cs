using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<JobApplication> ApplicationQuery()
        {
            return _context.JobApplications
                .Include(a => a.Student)
                    .ThenInclude(s => s.College)
                .Include(a => a.JobOpening)
                    .ThenInclude(j => j.Company);
        }

        public async Task<JobApplication> CreateAsync(
            JobApplication application)
        {
            await _context.JobApplications.AddAsync(application);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(application.JobApplicationId)
                ?? application;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await ApplicationQuery()
                .Include(a => a.StatusHistory)
                .FirstOrDefaultAsync(
                    a => a.JobApplicationId == id);
        }

        public async Task<JobApplication?> GetByStudentAndJobAsync(
            int studentId,
            int jobOpeningId)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(a =>
                    a.StudentId == studentId &&
                    a.JobOpeningId == jobOpeningId);
        }

        public async Task<List<JobApplication>> GetAllAsync()
        {
            return await ApplicationQuery()
                .AsNoTracking()
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }

        public async Task<List<JobApplication>> GetByStudentAsync(
            int studentId)
        {
            return await ApplicationQuery()
                .Where(a => a.StudentId == studentId)
                .AsNoTracking()
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }

        public async Task<List<JobApplication>> GetByJobAsync(
            int jobOpeningId)
        {
            return await ApplicationQuery()
                .Where(a => a.JobOpeningId == jobOpeningId)
                .AsNoTracking()
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }

        public async Task<JobApplication> UpdateAsync(
            JobApplication application)
        {
            await _context.SaveChangesAsync();

            return await GetByIdAsync(application.JobApplicationId)
                ?? application;
        }

        public async Task<List<ApplicationStatusHistory>> GetHistoryAsync(
            int jobApplicationId)
        {
            return await _context.ApplicationStatusHistories
                .Where(h => h.JobApplicationId == jobApplicationId)
                .AsNoTracking()
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task AddHistoryAsync(
            ApplicationStatusHistory history)
        {
            await _context.ApplicationStatusHistories.AddAsync(history);

            await _context.SaveChangesAsync();
        }
    }
}