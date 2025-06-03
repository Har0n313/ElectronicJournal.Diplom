using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class GradeService : IGradeService
{
    private readonly ApplicationDbContext _context;

    public GradeService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Grade> CreateGrade(int studentId, int lessonId, Grade grade)
    {
        if (grade == null)
        {
            throw new ArgumentNullException(nameof(grade));
        }

        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        var newGrade = new Grade
        {
            GradeValue = grade.GradeValue,
            Comment = grade.Comment,
            StudentId = studentId,
            LessonId = lessonId
        };

        _context.Grades.Add(newGrade);
        await _context.SaveChangesAsync();
        return newGrade;
    }

    public async Task<Grade> UpdateGrade(int id, Grade grade)
    {
        if (grade == null)
        {
            throw new ArgumentNullException(nameof(grade));
        }

        var existingGrade = await _context.Grades.FindAsync(id);
        if (existingGrade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {id} not found.");
        }

        existingGrade.GradeValue = grade.GradeValue;
        existingGrade.Comment = grade.Comment;

        _context.Grades.Update(existingGrade);
        await _context.SaveChangesAsync();
        return existingGrade;
    }

    public async Task<bool> DeleteGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {id} not found.");
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Grade>> GetGradesByStudent(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        return await _context.Grades
            .Where(g => g.StudentId == studentId)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Grade>> GetGradesByLesson(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        return await _context.Grades
            .Where(g => g.LessonId == lessonId)
            .Include(g => g.Student)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }
}