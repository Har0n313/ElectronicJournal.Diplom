using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class LessonService : ILessonService
{
    private readonly ApplicationDbContext _context;

    public LessonService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson> CreateLesson(Lesson lesson)
    {
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

        var subjectAssignment = await _context.SubjectAssignments.FindAsync(lesson.SubjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {lesson.SubjectAssignmentId} не найдено.");
        }

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateLesson(Lesson lesson)
    {
        if (lesson == null)
        {
            throw new ArgumentNullException(nameof(lesson));
        }

        var existingLesson = await _context.Lessons.FindAsync(lesson.Id);
        if (existingLesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {lesson.Id} не найдено.");
        }

        var subjectAssignment = await _context.SubjectAssignments.FindAsync(lesson.SubjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {lesson.SubjectAssignmentId} не найдено.");
        }

        existingLesson.Date = lesson.Date;
        existingLesson.LessonType = lesson.LessonType;
        existingLesson.Topic = lesson.Topic;
        existingLesson.SubjectAssignmentId = lesson.SubjectAssignmentId;

        _context.Lessons.Update(existingLesson);
        await _context.SaveChangesAsync();
        return existingLesson;
    }

    public async Task<bool> DeleteLesson(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Занятие с ID {id} не найдено.");
        }

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Lesson>> GetLessonsBySubjectAssignment(int subjectAssignmentId)
    {
        var subjectAssignment = await _context.SubjectAssignments.FindAsync(subjectAssignmentId);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {subjectAssignmentId} не найдено.");
        }

        var query = _context.Lessons
            .Where(l => l.SubjectAssignmentId == subjectAssignmentId)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Lesson>> GetLessonsByGroupAndDate(int groupId, DateTime date)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {groupId} не найдена.");
        }

        var query = _context.Lessons
            .Where(l => l.SubjectAssignment.GroupId == groupId && l.Date.Date == date.Date)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Teacher)
            .OrderBy(l => l.Date);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Lesson>> GetLessonsByTeacherAndDate(int teacherId, DateTime date)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = _context.Lessons
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
}