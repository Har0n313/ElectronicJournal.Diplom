using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ISubjectAssignmentService
{
    Task<SubjectAssignment> AssignSubject(SubjectAssignment dto);

    Task UnassignSubject(int id);

    Task<ICollection<SubjectAssignment>> GetAssignmentsByTeacher(int teacherId);

    Task<ICollection<SubjectAssignment>> GetAssignmentsByGroup(int groupId);

    Task<ICollection<SubjectAssignment>> GetAssignmentsBySemester(int semesterId);
}