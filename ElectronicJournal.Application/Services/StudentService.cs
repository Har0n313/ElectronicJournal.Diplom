using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class StudentService : IStudentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public StudentService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<Student> CreateStudent(Student student)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (student == null)
            throw new ArgumentNullException();

        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> GetStudentById(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students
            .Include(s => s.User)
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id);

        return student ?? throw new KeyNotFoundException($"Студент с ID {id} не найден.");
    }

    public async Task<ICollection<Student>> GetAllStudents()
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Students
            .Include(s => s.User)
            .Include(s => s.Group)
            .ToListAsync();
    }

    public async Task<Student> UpdateStudent(Student students)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students.FindAsync(students.Id);
        if (student == null)
            throw new KeyNotFoundException();

        student.FirstName = students.FirstName;
        student.LastName = students.LastName;
        student.MiddleName = students.MiddleName;
        student.GroupId = students.GroupId;

        await context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteStudent(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students.FindAsync(id);
        if (student == null) return false;

        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Assessment>> GetStudentGrades(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return await context.Assessments
            .Where(g => g.StudentId == id)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Attendance>> GetStudentAttendance(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var student = await context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return await context.Attendances
            .Where(a => a.StudentId == id)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }
}