using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class GroupService : IGroupService
{
    private readonly ApplicationDbContext _context;

    public GroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Group>> GetAllGroups()
    {
        return await _context.Groups.ToListAsync();
    }

    public Task<Group> GetGroupById(int id)
    {
        var group = _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found.");
        }

        return group;
    }

    public Task<Group> CreateGroup(Group group)
    {
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

        _context.Groups.Add(group);
        _context.SaveChanges();
        return Task.FromResult(group);
    }

    public async Task<Group> UpdateGroup(int id, Group group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var existingGroupe = await _context.Groups.FindAsync(id);
        if (existingGroupe == null)
        {
            throw new KeyNotFoundException($"Grade with ID {id} not found.");
        }

        existingGroupe.Name = group.Name;
        existingGroupe.SpecialtyCode = group.SpecialtyCode;
        existingGroupe.AdmissionYear = group.AdmissionYear;

        _context.Groups.Update(existingGroupe);
        await _context.SaveChangesAsync();
        return existingGroupe;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found.");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Student>> GetGroupStudents(int groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {groupId} not found.");
        }

        return await _context.Students
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }
}