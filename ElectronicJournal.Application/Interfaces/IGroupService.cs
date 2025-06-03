using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IGroupService
{
    Task<ICollection<Group>> GetAllGroups();

    Task<Group> GetGroupById(int id);

    Task<Group> CreateGroup(Group group);

    Task<Group> UpdateGroup(int id, Group group);

    Task<bool> DeleteGroup(int id);

    Task<ICollection<Student>> GetGroupStudents(int groupId);
}