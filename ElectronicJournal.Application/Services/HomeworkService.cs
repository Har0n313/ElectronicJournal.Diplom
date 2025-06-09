using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class HomeworkService : IHomeworkService
{
    private readonly ApplicationDbContext _context;

    public HomeworkService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Homework> CreateHomework(Homework homework)
    {
        if (homework == null)
        {
            throw new ArgumentNullException(nameof(homework));
        }

        if (string.IsNullOrWhiteSpace(homework.Description) || homework.DueDate == default)
        {
            throw new ArgumentException("Required fields (Description, DueDate) must not be empty or invalid.");
        }

        var lesson = await _context.Lessons.FindAsync(homework.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {homework.LessonId} not found.");
        }

        _context.Homeworks.Add(homework);
        await _context.SaveChangesAsync();
        return homework;
    }

    public async Task<Homework> UpdateHomework(Homework homework)
    {
        if (homework == null)
        {
            throw new ArgumentNullException(nameof(homework));
        }

        var existingHomework = await _context.Homeworks.FindAsync(homework.Id);
        if (existingHomework == null)
        {
            throw new KeyNotFoundException($"Homework with ID {homework.Id} not found.");
        }

        var lesson = await _context.Lessons.FindAsync(homework.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {homework.LessonId} not found.");
        }

        existingHomework.Description = homework.Description;
        existingHomework.DueDate = homework.DueDate;
        existingHomework.LessonId = homework.LessonId;

        _context.Homeworks.Update(existingHomework);
        await _context.SaveChangesAsync();
        return existingHomework;
    }

    public async Task<bool> DeleteHomework(int id)
    {
        var homework = await _context.Homeworks.FindAsync(id);
        if (homework == null)
        {
            throw new KeyNotFoundException($"Homework with ID {id} not found.");
        }

        _context.Homeworks.Remove(homework);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Homework>> GetHomeworkByLesson(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        return await _context.Homeworks
            .Where(h => h.LessonId == lessonId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Homework>> GetHomeworkByGroup(int groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {groupId} not found.");
        }

        return await _context.Homeworks
            .Where(h => h.Lesson.SubjectAssignment.GroupId == groupId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .OrderBy(h => h.DueDate)
            .ToListAsync();
    }

    public async Task<ICollection<Homework>> GetHomeworkByStudent(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        return await _context.Homeworks
            .Where(h => h.Lesson.SubjectAssignment.GroupId == student.GroupId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .OrderBy(h => h.DueDate)
            .ToListAsync();
    }
}