using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class LessonService : ILessonService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public LessonService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<Lesson> CreateLesson(Lesson lesson)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (lesson == null)
        {
            throw new ArgumentNullException(nameof(lesson));
        }

        if (string.IsNullOrWhiteSpace(lesson.LessonType) || string.IsNullOrWhiteSpace(lesson.Topic) ||
            lesson.Date == default)
        {
            throw new ArgumentException(
                "Обязательные поля (LessonType, Topic, Date) не могут быть пустыми или некорректными.");
        }

        var subjectAssignment = await context.SubjectAssignments.FindAsync(lesson.SubjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {lesson.SubjectAssignmentId} не найдено.");
        }

        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateLesson(Lesson lesson)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (lesson == null)
        {
            throw new ArgumentNullException(nameof(lesson));
        }

        var existingLesson = await context.Lessons.FindAsync(lesson.Id);
        if (existingLesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {lesson.Id} не найдено.");
        }

        var subjectAssignment = await context.SubjectAssignments.FindAsync(lesson.SubjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {lesson.SubjectAssignmentId} не найдено.");
        }

        existingLesson.Date = lesson.Date;
        existingLesson.LessonType = lesson.LessonType;
        existingLesson.Topic = lesson.Topic;
        existingLesson.SubjectAssignmentId = lesson.SubjectAssignmentId;

        context.Lessons.Update(existingLesson);
        await context.SaveChangesAsync();
        return existingLesson;
    }

    public async Task<bool> DeleteLesson(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var lesson = await context.Lessons.FindAsync(id);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {id} не найдено.");
        }

        context.Lessons.Remove(lesson);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Lesson> GetLessonById(int id)
    {
        await using var context = await _context.CreateDbContextAsync();
        
        var lesson = await context.Lessons.FindAsync(id);
        if (lesson == null)
            throw new Exception("Такого урока не существует");
        return lesson;
    }

    public async Task<ICollection<Lesson>> GetLessonsBySubjectAssignment(int subjectAssignmentId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var subjectAssignment = await context.SubjectAssignments.FindAsync(subjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {subjectAssignmentId} не найдено.");
        }

        var query = context.Lessons
            .Where(l => l.SubjectAssignmentId == subjectAssignmentId)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<Lesson?> GetLessonsByGroupSubjectAndDate(int groupId, int subjectId, DateTime date)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {groupId} не найдена.");
        }

        // Убедимся, что дата в UTC
        var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

        var query = context.Lessons
            .Where(l =>
                l.SubjectAssignment.GroupId == groupId &&
                l.Date.Date == utcDate &&
                l.SubjectAssignmentId == subjectId)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Teacher)
            .FirstOrDefaultAsync();
        return await query;
    }

    public async Task<ICollection<Lesson>> GetLessonsByTeacherAndDate(int teacherId, DateTime date)
    {
        await using var context = await _context.CreateDbContextAsync();

        var teacher = await context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = context.Lessons
            .Where(l => l.SubjectAssignment.TeacherId == teacherId && l.Date.Date == date.Date)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Teacher)
            .OrderBy(l => l.Date);

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<Lesson>> GetLessonsBySubjectAndGroup(int groupId, int subjectId)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Lessons
            .Include(l => l.SubjectAssignment)
            .Where(l => l.SubjectAssignment.GroupId == groupId && l.SubjectAssignment.SubjectId == subjectId)
            .ToListAsync();
    }

}