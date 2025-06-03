using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _context;

    public SubjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Subject>> GetAllSubjects()
    {
        var query = _context.Subjects
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Semester);

        return await query.ToListAsync();
    }

    public async Task<Subject> GetSubjectById(int id)
    {
        var query = _context.Subjects
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Semester);

        var subject = await query.FirstOrDefaultAsync(s => s.Id == id);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {id} не найден.");
        }

        return subject;
    }

    public async Task<Subject> CreateSubject(Subject subject)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (string.IsNullOrWhiteSpace(subject.Name) || string.IsNullOrWhiteSpace(subject.Code))
        {
            throw new ArgumentException("Обязательные поля (Name, Code) не могут быть пустыми.");
        }

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject> UpdateSubject(int id, Subject subject)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        var existingSubject = await _context.Subjects.FindAsync(id);
        if (existingSubject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {id} не найден.");
        }

        existingSubject.Name = subject.Name;
        existingSubject.Code = subject.Code;

        _context.Subjects.Update(existingSubject);
        await _context.SaveChangesAsync();
        return existingSubject;
    }

    public async Task<bool> DeleteSubject(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {id} не найден.");
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return true;
    }
}