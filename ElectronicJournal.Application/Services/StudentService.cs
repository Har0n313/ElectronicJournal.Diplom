using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> CreateStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException();

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> GetStudentById(int id)
    {
        return await _context.Students.FindAsync(id) ?? throw new InvalidOperationException();
        /*.Include(s => s.User)
        .Include(s => s.Group)
        .FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new KeyNotFoundException();*/
    }

    public async Task<ICollection<Student>> GetAllStudents()
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.Group)
            .ToListAsync();
    }

    public async Task<Student> UpdateStudent(Student students)
    {
        var student = await _context.Students.FindAsync(students.Id);
        if (student == null)
            throw new KeyNotFoundException();

        student.FirstName = students.FirstName;
        student.LastName = students.LastName;
        student.MiddleName = students.MiddleName;
        student.GroupId = students.GroupId;

        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Assessment>> GetStudentGrades(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return await _context.Assessments
            .Where(g => g.StudentId == id)
            .Include(g => g.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<ICollection<Attendance>> GetStudentAttendance(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return await _context.Attendances
            .Where(a => a.StudentId == id)
            .Include(a => a.Lesson)
            .ThenInclude(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .ToListAsync();
    }

    public async Task<Semester> GetStudentCurrentSemester(int id)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        var currentDate = DateTime.UtcNow;
        return await _context.Semesters
            .Where(s => s.StartDate <= currentDate && s.EndDate >= currentDate)
            .FirstOrDefaultAsync();
    }
}
