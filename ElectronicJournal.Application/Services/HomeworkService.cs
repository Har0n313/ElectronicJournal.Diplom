using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class HomeworkService : IHomeworkService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public HomeworkService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<Homework> CreateHomework(Homework homework)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (homework == null)
        {
            throw new ArgumentNullException(nameof(homework));
        }

        if (string.IsNullOrWhiteSpace(homework.Description) || homework.DueDate == default)
        {
            throw new ArgumentException("Required fields (Description, DueDate) must not be empty or invalid.");
        }

        var lesson = await context.Lessons.FindAsync(homework.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {homework.LessonId} not found.");
        }

        context.Homeworks.Add(homework);
        await context.SaveChangesAsync();
        return homework;
    }

    public async Task<Homework> UpdateHomework(Homework homework)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (homework == null)
        {
            throw new ArgumentNullException(nameof(homework));
        }

        var existingHomework = await context.Homeworks.FindAsync(homework.Id);
        if (existingHomework == null)
        {
            throw new KeyNotFoundException($"Homework with ID {homework.Id} not found.");
        }

        var lesson = await context.Lessons.FindAsync(homework.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {homework.LessonId} not found.");
        }

        existingHomework.Description = homework.Description;
        existingHomework.DueDate = homework.DueDate;
        existingHomework.LessonId = homework.LessonId;

        context.Homeworks.Update(existingHomework);
        await context.SaveChangesAsync();
        return existingHomework;
    }

    public async Task<bool> DeleteHomework(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var homework = await context.Homeworks.FindAsync(id);
        if (homework == null)
        {
            throw new KeyNotFoundException($"Homework with ID {id} not found.");
        }

        context.Homeworks.Remove(homework);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Homework>> GetHomeworkByLesson(int lessonId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var lesson = await context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        return await context.Homeworks
            .Where(h => h.LessonId == lessonId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Homework>> GetHomeworkByGroup(int groupId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {groupId} not found.");
        }

        return await context.Homeworks
            .Where(h => h.Lesson.SubjectAssignment.GroupId == groupId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .OrderBy(h => h.DueDate)
            .ToListAsync();
    }

    public async Task<ICollection<Homework>> GetHomeworkByStudent(int studentId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        return await context.Homeworks
            .Where(h => h.Lesson.SubjectAssignment.GroupId == student.GroupId)
            .Include(h => h.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .OrderBy(h => h.DueDate)
            .ToListAsync();
    }
}