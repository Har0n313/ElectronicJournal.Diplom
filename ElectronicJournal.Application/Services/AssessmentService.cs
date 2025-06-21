using ElectronicJournal.Application.dto;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public AssessmentService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<Assessment> CreateAssessment(Assessment assessment)
    {
        await using var context = await _context.CreateDbContextAsync();

        var newGrade = new Assessment
        {
            MarkValue = assessment.MarkValue,
            StudentId = assessment.StudentId,
            LessonId = assessment.LessonId,
        };

        context.Assessments.Add(newGrade);
        await context.SaveChangesAsync();
        return newGrade;
    }

    public async Task<Assessment> UpdateAssessments(Assessment assessment)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (assessment == null)
        {
            throw new ArgumentNullException(nameof(assessment));
        }

        var existingGrade = await context.Assessments.FindAsync(assessment.Id);
        if (existingGrade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {assessment.Id} not found.");
        }

        existingGrade.MarkValue = assessment.MarkValue;

        context.Assessments.Update(existingGrade);
        await context.SaveChangesAsync();
        return existingGrade;
    }

    public async Task<bool> DeleteAssessment(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var grade = await context.Assessments.FindAsync(id);
        if (grade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {id} not found.");
        }

        context.Assessments.Remove(grade);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Assessment>> GetGradesByStudent(int studentId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students.FindAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        return await context.Assessments
            .Where(g => g.StudentId == studentId)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Assessment>> GetAssessmentByLesson(int lessonId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var lesson = await context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        return await context.Assessments
            .Where(g => g.LessonId == lessonId)
            .Include(g => g.Student)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssessmentDto>> GetAssessmentsByClassAndDiscipline(string className, int subjectId)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name cannot be empty", nameof(className));

        var assessments = await context.Assessments
            .Include(a => a.Student)
            .ThenInclude(s => s.Group)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Where(a =>
                a.Student.Group.Name == className &&
                a.Lesson.SubjectAssignment.SubjectId == subjectId)
            .ToListAsync();

        return assessments.Select(a => new AssessmentDto
        {
            Id = a.Id,
            StudentId = a.StudentId,
            LessonId = a.LessonId,
            GradeValue = a.MarkValue,
        }).ToList();
    }
}