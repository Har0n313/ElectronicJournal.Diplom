using System.Collections;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IGroupService
{
    Task<ICollection<Group>> GetAllAsync();

    Task<Group> GetGroupById(int id);

    Task<Group> CreateGroup(Group group);

    Task<Group> UpdateGroup(Group group);

    Task<bool> DeleteGroup(int id);
    
    Task<Group> GetGroupByName(string name);

    Task<ICollection<Student>> GetGroupStudents(int groupId);
}