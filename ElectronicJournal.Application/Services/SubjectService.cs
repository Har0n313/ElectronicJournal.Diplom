using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public SubjectService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<ICollection<Subject>> GetAllSubjects()
    {
        await using var context = await _context.CreateDbContextAsync();

        var query = context.Subjects
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher)
            .Include(s => s.SubjectAssignments);

        return await query.ToListAsync();
    }

    public async Task<Subject> GetSubjectById(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var query = context.Subjects
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher)
            .Include(s => s.SubjectAssignments);

        var subject = await query.FirstOrDefaultAsync(s => s.Id == id);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {id} не найден.");
        }

        return subject;
    }

    public async Task<Subject> CreateSubject(Subject subject)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (string.IsNullOrWhiteSpace(subject.Name) || string.IsNullOrWhiteSpace(subject.Code))
        {
            throw new ArgumentException("Обязательные поля (Name, Code) не могут быть пустыми.");
        }

        context.Subjects.Add(subject);
        await context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject> UpdateSubject(Subject subject)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        var existingSubject = await context.Subjects.FindAsync(subject.Id);
        if (existingSubject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {subject.Id} не найден.");
        }

        existingSubject.Name = subject.Name;
        existingSubject.Code = subject.Code;

        context.Subjects.Update(existingSubject);
        await context.SaveChangesAsync();
        return existingSubject;
    }

    public async Task<ICollection<Subject>> GetSubjectsByTeacher(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var subjects = await context.Subjects
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher)
            .Include(s => s.SubjectAssignments)
            .Where(s => s.SubjectAssignments.Any(sa => sa.TeacherId == id))
            .ToListAsync();

        if (subjects == null || !subjects.Any())
        {
            throw new KeyNotFoundException($"Предметы для преподавателя с ID {id} не найдены.");
        }

        return subjects;
    }


    public async Task<bool> DeleteSubject(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var subject = await context.Subjects.FindAsync(id);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {id} не найден.");
        }

        context.Subjects.Remove(subject);
        await context.SaveChangesAsync();
        return true;
    }
}