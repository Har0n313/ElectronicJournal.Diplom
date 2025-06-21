using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public TeacherService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<Teacher> CreateTeacher(Teacher teacher)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (teacher == null)
            throw new ArgumentNullException(nameof(teacher));

        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
        return teacher;
    }

    public async Task<Teacher> GetTeacherById(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Teachers
                   .Include(t => t.SubjectAssignments)
                   .ThenInclude(sa => sa.Subject)
                   .Include(t => t.User)
                   .FirstOrDefaultAsync(t => t.Id == id)
               ?? throw new KeyNotFoundException($"Teacher with ID {id} not found.");
    }

    public async Task<ICollection<Teacher>> GetAllTeachers()
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Teachers
            .Include(t => t.SubjectAssignments)
            .ThenInclude(sa => sa.Subject)
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<Teacher> UpdateTeacher(Teacher updated)
    {
        await using var context = await _context.CreateDbContextAsync();

        var teacher = await context.Teachers.FindAsync(updated.Id);
        if (teacher == null)
            throw new KeyNotFoundException();

        teacher.FirstName = updated.FirstName;
        teacher.LastName = updated.LastName;
        teacher.MiddleName = updated.MiddleName;

        await context.SaveChangesAsync();
        return teacher;
    }

    public async Task<bool> DeleteTeacher(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var teacher = await context.Teachers.FindAsync(id);
        if (teacher == null) return false;

        context.Teachers.Remove(teacher);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Subject>> GetAssignedSubjects(int teacherId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var teacher = await context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = context.SubjectAssignments
            .Where(sa => sa.TeacherId == teacherId)
            .Include(sa => sa.Subject)
            .Select(sa => sa.Subject);

        return await query.ToListAsync();
    }

    public async Task<ICollection<Lesson>> GetTeacherSchedule(int teacherId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var teacher = await context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = context.Lessons
            .Where(l => l.SubjectAssignment.TeacherId == teacherId)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Subject)
            .Include(l => l.SubjectAssignment)
            .ThenInclude(sa => sa.Group)
            .Include(l => l.SubjectAssignment)
            .OrderBy(l => l.Date);

        return await query.ToListAsync();
    }
}