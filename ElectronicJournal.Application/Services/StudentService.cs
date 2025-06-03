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

    public async Task<ICollection<Student>> GetAllStudents()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student> GetStudentById(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return student;
    }

    public async Task<Student> CreateStudent(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (string.IsNullOrWhiteSpace(student.LastName) || string.IsNullOrWhiteSpace(student.FirstName) ||
            string.IsNullOrWhiteSpace(student.RecordBookNo) || string.IsNullOrWhiteSpace(student.Status))
        {
            throw new ArgumentException(
                "Required fields (LastName, FirstName, RecordBookNo, Status) must not be empty.");
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateStudent(int id, Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        var existingStudent = await _context.Students.FindAsync(id);
        if (existingStudent == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        existingStudent.FirstName = student.FirstName;
        existingStudent.LastName = student.LastName;
        existingStudent.MiddleName = student.MiddleName;
        existingStudent.BirthDate = student.BirthDate;
        existingStudent.RecordBookNo = student.RecordBookNo;
        existingStudent.Status = student.Status;
        existingStudent.GroupId = student.GroupId;

        _context.Students.Update(existingStudent);
        await _context.SaveChangesAsync();
        return existingStudent;
    }

    public async Task<bool> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Grade>> GetStudentGrades(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        return await _context.Grades
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