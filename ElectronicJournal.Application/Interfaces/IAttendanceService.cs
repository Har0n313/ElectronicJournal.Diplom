using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IAttendanceService
{
    Task<Attendance> MarkAttendance(Attendance attendance);

    Task<Attendance> UpdateAttendance(int id, Attendance attendance);

    Task<ICollection<Attendance>> GetAttendanceByLesson(int lessonId);

    Task<ICollection<Attendance>> GetAttendanceByStudent(int studentId);

    Task<ICollection<Attendance>> GetAttendanceByDate(DateTime date);
}