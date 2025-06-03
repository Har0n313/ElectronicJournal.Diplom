using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class SubjectAssignmentService : ISubjectAssignmentService
{
    private readonly ApplicationDbContext _context;

    public SubjectAssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectAssignment> AssignSubject(SubjectAssignment subjectAssignment)
    {
        if (subjectAssignment == null)
        {
            throw new ArgumentNullException(nameof(subjectAssignment));
        }

        var group = await _context.Groups.FindAsync(subjectAssignment.GroupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {subjectAssignment.GroupId} не найдена.");
        }

        var subject = await _context.Subjects.FindAsync(subjectAssignment.SubjectId);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {subjectAssignment.SubjectId} не найден.");
        }

        var teacher = await _context.Teachers.FindAsync(subjectAssignment.TeacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {subjectAssignment.TeacherId} не найден.");
        }

        var semester = await _context.Semesters.FindAsync(subjectAssignment.SemesterId);
        if (semester == null)
        {
            throw new KeyNotFoundException($"Семестр с ID {subjectAssignment.SemesterId} не найден.");
        }

        _context.SubjectAssignments.Add(subjectAssignment);
        await _context.SaveChangesAsync();
        return subjectAssignment;
    }

    public async Task UnassignSubject(int id)
    {
        var subjectAssignment = await _context.SubjectAssignments.FindAsync(id);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {id} не найдено.");
        }

        _context.SubjectAssignments.Remove(subjectAssignment);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<SubjectAssignment>> GetAssignmentsByTeacher(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
        }

        var query = _context.SubjectAssignments
            .Where(sa => sa.TeacherId == teacherId)
            .Include(sa => sa.Subject)
            .Include(sa => sa.Group)
            .Include(sa => sa.Semester)
            .Include(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<ICollection<SubjectAssignment>> GetAssignmentsByGroup(int groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {groupId} не найдена.");
        }

        var query = _context.SubjectAssignments
            .Where(sa => sa.GroupId == groupId)
            .Include(sa => sa.Subject)
            .Include(sa => sa.Group)
            .Include(sa => sa.Semester)
            .Include(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<ICollection<SubjectAssignment>> GetAssignmentsBySemester(int semesterId)
    {
        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
        {
            throw new KeyNotFoundException($"Семестр с ID {semesterId} не найден.");
        }

        var query = _context.SubjectAssignments
            .Where(sa => sa.SemesterId == semesterId)
            .Include(sa => sa.Subject)
            .Include(sa => sa.Group)
            .Include(sa => sa.Semester)
            .Include(sa => sa.Teacher);

        return await query.ToListAsync();
    }
}