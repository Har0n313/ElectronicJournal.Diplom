using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;

    public TeacherService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Teacher>> GetAllTeachers()
    {
        var query = _context.Teachers
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Subject)
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Semester);

        return await query.ToListAsync();
    }

    public async Task<Teacher> GetTeacherById(int id)
    {
        var query = _context.Teachers
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Subject)
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Semester);

        var teacher = await query.FirstOrDefaultAsync(t => t.Id == id);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {id} не найден.");
        }

        return teacher;
    }

    public async Task<Teacher> CreateTeacher(Teacher teacher)
    {
        if (teacher == null)
        {
            throw new ArgumentNullException(nameof(teacher));
        }

        if (string.IsNullOrWhiteSpace(teacher.FirstName) || string.IsNullOrWhiteSpace(teacher.LastName) ||
            string.IsNullOrWhiteSpace(teacher.Position))
        {
            throw new ArgumentException("Обязательные поля (FirstName, LastName, Position) не могут быть пустыми.");
        }

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        return teacher;
    }

    public async Task<Teacher> UpdateTeacher(int id, Teacher teacher)
    {
        if (teacher == null)
        {
            throw new ArgumentNullException(nameof(teacher));
        }

        var existingTeacher = await _context.Teachers.FindAsync(id);
        if (existingTeacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {id} не найден.");
        }

        existingTeacher.FirstName = teacher.FirstName;
        existingTeacher.LastName = teacher.LastName;
        existingTeacher.MiddleName = teacher.MiddleName;
        existingTeacher.Position = teacher.Position;

        _context.Teachers.Update(existingTeacher);
        await _context.SaveChangesAsync();
        return existingTeacher;
    }

    public async Task<bool> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {id} не найден.");
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Subject>> GetAssignedSubjects(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = _context.SubjectAssignments
            .Where(sa => sa.TeacherId == teacherId)
            .Include(sa => sa.Subject)
            .Select(sa => sa.Subject);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Lesson>> GetTeacherSchedule(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = _context.Lessons
            .Where(l => l.SubjectAssignment.TeacherId == teacherId)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Semester)
            .OrderBy(l => l.Date);

        return await query.ToListAsync();
    }
}