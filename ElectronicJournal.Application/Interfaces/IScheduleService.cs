using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<Schedule>> GetScheduleForGroup(int groupId);
    Task<IEnumerable<Schedule>> GetScheduleForTeacher(int teacherId);
    Task<IEnumerable<Schedule>> GetScheduleByDay(int groupId, DayOfWeek day);
    
    Task AddSchedule(Schedule entry);
    Task UpdateSchedule(Schedule entry);
    Task DeleteSchedule(int entryId);

    Task<IEnumerable<TimeSpan>> GetPairTimes(int pairNumber);
}