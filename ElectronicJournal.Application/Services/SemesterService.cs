using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class SemesterService : ISemesterService
{
    private readonly ApplicationDbContext _context;

    public SemesterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Semester>> GetAllSemesters()
    {
        var query = _context.Semesters
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Subject)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<Semester> GetSemesterById(int id)
    {
        var query = _context.Semesters
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Subject)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Group)
            .Include(s => s.SubjectAssignments)
            .ThenInclude(sa => sa.Teacher);

        var semester = await query.FirstOrDefaultAsync(s => s.Id == id);
        if (semester == null)
        {
            throw new KeyNotFoundException($"Семестр с ID {id} не найден.");
        }

        return semester;
    }

    public async Task<Semester> CreateSemester(Semester semester)
    {
        if (semester == null)
        {
            throw new ArgumentNullException(nameof(semester));
        }

        if (string.IsNullOrWhiteSpace(semester.AcademicYear) || semester.Number <= 0 || semester.StartDate == default ||
            semester.EndDate == default)
        {
            throw new ArgumentException(
                "Обязательные поля (AcademicYear, Number, StartDate, EndDate) не могут быть пустыми или некорректными.");
        }

        if (semester.StartDate >= semester.EndDate)
        {
            throw new ArgumentException("Дата начала семестра должна быть раньше даты окончания.");
        }

        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();
        return semester;
    }

    public async Task<Semester> UpdateSemester(int id, Semester semester)
    {
        if (semester == null)
        {
            throw new ArgumentNullException(nameof(semester));
        }

        var existingSemester = await _context.Semesters.FindAsync(id);
        if (existingSemester == null)
        {
            throw new KeyNotFoundException($"Семестр с ID {id} не найден.");
        }

        existingSemester.Number = semester.Number;
        existingSemester.AcademicYear = semester.AcademicYear;
        existingSemester.StartDate = semester.StartDate;
        existingSemester.EndDate = semester.EndDate;

        if (existingSemester.StartDate >= existingSemester.EndDate)
        {
            throw new ArgumentException("Дата начала семестра должна быть раньше даты окончания.");
        }

        _context.Semesters.Update(existingSemester);
        await _context.SaveChangesAsync();
        return existingSemester;
    }

    public async Task<bool> DeleteSemester(int id)
    {
        var semester = await _context.Semesters.FindAsync(id);
        if (semester == null)
        {
            throw new KeyNotFoundException($"Семестр с ID {id} не найден.");
        }

        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync();
        return true;
    }
}