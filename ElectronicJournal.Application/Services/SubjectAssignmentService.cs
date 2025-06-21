using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class SubjectAssignmentService : ISubjectAssignmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public SubjectAssignmentService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<ICollection<SubjectAssignment>> GetAllAsync()
    {
        await using var context = await _context.CreateDbContextAsync();
        return await context.SubjectAssignments
            .Include(sa => sa.Teacher)
            .Include(sa => sa.Subject)
            .Include(sa => sa.Group)
            .ToListAsync();
    }


    public async Task<SubjectAssignment> AssignSubject(SubjectAssignment subjectAssignment)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (subjectAssignment == null)
        {
            throw new ArgumentNullException(nameof(subjectAssignment));
        }

        var group = await context.Groups.FindAsync(subjectAssignment.GroupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {subjectAssignment.GroupId} не найдена.");
        }

        var subject = await context.Subjects.FindAsync(subjectAssignment.SubjectId);
        if (subject == null)
        {
            throw new KeyNotFoundException($"Предмет с ID {subjectAssignment.SubjectId} не найден.");
        }

        var teacher = await context.Teachers.FindAsync(subjectAssignment.TeacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {subjectAssignment.TeacherId} не найден.");
        }

        context.SubjectAssignments.Add(subjectAssignment);
        await context.SaveChangesAsync();
        return subjectAssignment;
    }

    public async Task UnassignSubject(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var subjectAssignment = await context.SubjectAssignments.FindAsync(id);
        if (subjectAssignment == null)
        {
            throw new KeyNotFoundException($"Назначение предмета с ID {id} не найдено.");
        }

        context.SubjectAssignments.Remove(subjectAssignment);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<SubjectAssignment>> GetAssignmentsByTeacher(int teacherId)
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
            .Include(sa => sa.Group)
            .Include(sa => sa.Teacher);

        return await query.ToListAsync();
    }

    public async Task<ICollection<SubjectAssignment>> GetAssignmentsByGroup(int groupId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Группа с ID {groupId} не найдена.");
        }

        var query = context.SubjectAssignments
            .Where(sa => sa.GroupId == groupId)
            .Include(sa => sa.Subject)
            .Include(sa => sa.Group)
            .Include(sa => sa.Teacher);

        return await query.ToListAsync();
    }
}