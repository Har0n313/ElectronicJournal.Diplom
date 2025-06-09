using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;

    public AttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance> MarkAttendance(Attendance attendance)
    {
        if (attendance == null)
        {
            throw new ArgumentNullException(nameof(attendance));
        }

        var student = await _context.Students.FindAsync(attendance.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Студент с ID {attendance.StudentId} не найден.");
        }

        var lesson = await _context.Lessons.FindAsync(attendance.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {attendance.LessonId} не найдено.");
        }

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task<Attendance> UpdateAttendance(Attendance attendance)
    {
        if (attendance == null)
        {
            throw new ArgumentNullException(nameof(attendance));
        }

        var existingAttendance = await _context.Attendances.FindAsync(attendance.Id);
        if (existingAttendance == null)
        {
            throw new KeyNotFoundException($"Запись о посещаемости с ID {attendance.Id} не найдена.");
        }

        var student = await _context.Students.FindAsync(attendance.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Студент с ID {attendance.StudentId} не найден.");
        }

        var lesson = await _context.Lessons.FindAsync(attendance.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {attendance.LessonId} не найдено.");
        }

        existingAttendance.Type = attendance.Type;
        existingAttendance.Date = attendance.Date;
        existingAttendance.StudentId = attendance.StudentId;
        existingAttendance.LessonId = attendance.LessonId;

        _context.Attendances.Update(existingAttendance);
        await _context.SaveChangesAsync();
        return existingAttendance;
    }

    public async Task<ICollection<Attendance>> GetAttendanceByLesson(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {lessonId} не найдено.");
        }

        var query = _context.Attendances
            .Where(a => a.LessonId == lessonId)
            .Include(a => a.Student)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Attendance>> GetAttendanceByStudent(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Студент с ID {studentId} не найден.");
        }

        var query = _context.Attendances
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Student)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Attendance>> GetAttendanceByDate(DateTime date)
    {
        var query = _context.Attendances
            .Where(a => a.Lesson.Date.Date == date.Date)
            .Include(a => a.Student)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject);

        return await query.ToListAsync();
    }
}