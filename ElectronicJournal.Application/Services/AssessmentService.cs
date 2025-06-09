using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class AssessmentService : IAssessmentService
{
    private readonly ApplicationDbContext _context;

    public AssessmentService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Assessment> CreateGrade(Assessment assessment)
    {
        var newGrade = new Assessment
        {
            GradeValue = assessment.GradeValue,
            StudentId = assessment.StudentId,
            LessonId = assessment.LessonId,
        };

        _context.Assessments.Add(newGrade);
        await _context.SaveChangesAsync();
        return newGrade;
    }

    public async Task<Assessment> UpdateAssessments(Assessment assessment)
    {
        if (assessment == null)
        {
            throw new ArgumentNullException(nameof(assessment));
        }

        var existingGrade = await _context.Assessments.FindAsync(assessment.Id);
        if (existingGrade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {assessment.Id} not found.");
        }

        existingGrade.GradeValue = assessment.GradeValue;

        _context.Assessments.Update(existingGrade);
        await _context.SaveChangesAsync();
        return existingGrade;
    }

    public async Task<bool> DeleteGrade(int id)
    {
        var grade = await _context.Assessments.FindAsync(id);
        if (grade == null)
        {
            throw new KeyNotFoundException($"Grade with ID {id} not found.");
        }

        _context.Assessments.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Assessment>> GetGradesByStudent(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }

        return await _context.Assessments
            .Where(g => g.StudentId == studentId)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Assessment>> GetGradesByLesson(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found.");
        }

        return await _context.Assessments
            .Where(g => g.LessonId == lessonId)
            .Include(g => g.Student)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }
    public async Task<IEnumerable<Assessment>> GetAssessmentsByClassAndDiscipline(string className, int disciplineId)
    {
        var assessments = await _context.Assessments
            .Include(a => a.Student)
            .ThenInclude(s => s.Group)
            .Include(a => a.Type)
            .Include(a => a.Lesson)
            .Where(a => a.Student.Group.Name == className && a.LessonId == disciplineId)
            .ToListAsync();

        return assessments.Select(a => new Assessment
        {
            Id = a.Id,
            StudentId = a.StudentId,
            LessonId = a.LessonId,
            GradeValue = a.GradeValue,
            DateCreated = a.DateCreated,
        });
    }

}