using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public ScheduleService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetScheduleForGroup(int groupId)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Schedules
            .Include(s => s.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(s => s.SubjectAssignment.Teacher)
            .ThenInclude(t => t.User)
            .Where(s => s.SubjectAssignment.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetScheduleForTeacher(int teacherId)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Schedules
            .Include(s => s.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Where(s => s.SubjectAssignment.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetScheduleByDay(int groupId, DayOfWeek day)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Schedules
            .Include(s => s.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(s => s.SubjectAssignment.Teacher)
            .ThenInclude(t => t.User)
            .Where(s => s.SubjectAssignment.GroupId == groupId && s.Day == day)
            .OrderBy(s => s.PairNumber)
            .ToListAsync();
    }

    public async Task AddSchedule(Schedule entry)
    {
        await using var context = await _context.CreateDbContextAsync();

        context.Schedules.Add(entry);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSchedule(Schedule entry)
    {
        await using var context = await _context.CreateDbContextAsync();

        context.Schedules.Update(entry);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSchedule(int entryId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var entry = await context.Schedules.FindAsync(entryId);
        if (entry != null)
        {
            context.Schedules.Remove(entry);
            await context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<string>> GetPairTimes(int pairNumber)
    {
        var times = new Dictionary<int, (string start, string end)>
        {
            [1] = ("8:30","9:50"),
            [2] = ("10:00","11:20"),
            [3] = ("11:30","12:50"),
            [4] = ("13:20","14:40"),
            [5] = ("14:50","16:10"),
            [6] = ("16:20","17:40"),
            [7] = ("17:50","18:10"),
            [8] = ("18:20","19:40"),
        };

        if (times.TryGetValue(pairNumber, out var time))
            return Task.FromResult<IEnumerable<string>>([time.start, time.end]);

        return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
    }
}