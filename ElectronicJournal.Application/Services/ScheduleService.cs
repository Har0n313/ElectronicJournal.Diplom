using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly ApplicationDbContext _context;

    public ScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetScheduleForGroup(int groupId)
    {
        return await _context.Schedules
            .Include(s => s.SubjectAssignment)
                .ThenInclude(sa => sa.Subject)
            .Include(s => s.SubjectAssignment.Teacher)
                .ThenInclude(t => t.User)
            .Where(s => s.SubjectAssignment.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetScheduleForTeacher(int teacherId)
    {
        return await _context.Schedules
            .Include(s => s.SubjectAssignment)
                .ThenInclude(sa => sa.Subject)
            .Where(s => s.SubjectAssignment.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetScheduleByDay(int groupId, DayOfWeek day)
    {
        return await _context.Schedules
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
        _context.Schedules.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSchedule(Schedule entry)
    {
        _context.Schedules.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSchedule(int entryId)
    {
        var entry = await _context.Schedules.FindAsync(entryId);
        if (entry != null)
        {
            _context.Schedules.Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<TimeSpan>> GetPairTimes(int pairNumber)
    {
        // Пример фиксированного расписания
        var times = new Dictionary<int, (TimeSpan start, TimeSpan end)>
        {
            [1] = (new TimeSpan(8, 30, 0), new TimeSpan(10, 0, 0)),
            [2] = (new TimeSpan(10, 10, 0), new TimeSpan(11, 40, 0)),
            [3] = (new TimeSpan(12, 10, 0), new TimeSpan(13, 40, 0)),
            [4] = (new TimeSpan(13, 50, 0), new TimeSpan(15, 20, 0)),
            [5] = (new TimeSpan(15, 30, 0), new TimeSpan(17, 0, 0))
        };

        if (times.TryGetValue(pairNumber, out var time))
            return Task.FromResult<IEnumerable<TimeSpan>>([time.start, time.end]);

        return Task.FromResult<IEnumerable<TimeSpan>>(Array.Empty<TimeSpan>());
    }
}
