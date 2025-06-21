using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class GroupService : IGroupService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public GroupService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }


    public async Task<ICollection<Group>> GetAllAsync()
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Groups.ToListAsync();
    }

    public async Task<Group> GetGroupById(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found.");
        }

        return group;
    }

    public async Task<Group> CreateGroup(Group group)
    { 
        await using var context = await _context.CreateDbContextAsync();
        
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        if (string.IsNullOrWhiteSpace(group.Name) || string.IsNullOrWhiteSpace(group.SpecialtyCode) ||
            group.AdmissionYear <= 0)
        {
            throw new ArgumentException(
                "Required fields (Name, SpecialtyCode, AdmissionYear) must not be empty or invalid.");
        }

        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();
        return group;
    }

    public async Task<Group> UpdateGroup(Group group)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var existingGroupe = await context.Groups.FindAsync(group.Id);
        if (existingGroupe == null)
        {
            throw new KeyNotFoundException($"Grade with ID {group.Id} not found.");
        }

        existingGroupe.Name = group.Name;
        existingGroupe.NameSpecialty = group.NameSpecialty;
        existingGroupe.SpecialtyCode = group.SpecialtyCode;
        existingGroupe.AdmissionYear = group.AdmissionYear;

        context.Groups.Update(existingGroupe);
        await context.SaveChangesAsync();
        return existingGroupe;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FindAsync(id);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found.");
        }

        context.Groups.Remove(group);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Group> GetGroupByName(string name)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Groups.FirstOrDefaultAsync(g => g.Name == name) ?? throw new InvalidOperationException();
    }


    public async Task<ICollection<Student>> GetGroupStudents(int groupId)
    {
        await using var context = await _context.CreateDbContextAsync();

        var group = await context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {groupId} not found.");
        }

        return await context.Students
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }
}