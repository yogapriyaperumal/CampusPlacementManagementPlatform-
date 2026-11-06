using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class InterviewRoundRepository : IInterviewRoundRepository
    {
        private readonly ApplicationDbContext _context;

        public InterviewRoundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<InterviewRound> RoundQuery()
        {
            return _context.InterviewRounds
                .Include(r => r.RecruitmentDrive);
        }

        public async Task<InterviewRound> CreateAsync(InterviewRound round)
        {
            _context.InterviewRounds.Add(round);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(round.InterviewRoundId)
                ?? throw new InvalidOperationException(
                    "Interview round could not be retrieved.");
        }

        public async Task<List<InterviewRound>> GetAllAsync()
        {
            return await RoundQuery()
                .AsNoTracking()
                .OrderBy(r => r.RecruitmentDriveId)
                .ThenBy(r => r.RoundNumber)
                .ToListAsync();
        }

        public async Task<InterviewRound?> GetByIdAsync(int id)
        {
            return await RoundQuery()
                .FirstOrDefaultAsync(r => r.InterviewRoundId == id);
        }

        public async Task<List<InterviewRound>> GetByDriveAsync(int driveId)
        {
            return await RoundQuery()
                .AsNoTracking()
                .Where(r => r.RecruitmentDriveId == driveId)
                .OrderBy(r => r.RoundNumber)
                .ToListAsync();
        }

        public async Task<bool> RoundNumberExistsAsync(
            int driveId,
            int roundNumber,
            int? excludeRoundId = null)
        {
            var query = _context.InterviewRounds
                .Where(r =>
                    r.RecruitmentDriveId == driveId &&
                    r.RoundNumber == roundNumber);

            if (excludeRoundId.HasValue)
            {
                query = query.Where(
                    r => r.InterviewRoundId != excludeRoundId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task UpdateAsync(InterviewRound round)
        {
            await _context.SaveChangesAsync();
        }
    }
}