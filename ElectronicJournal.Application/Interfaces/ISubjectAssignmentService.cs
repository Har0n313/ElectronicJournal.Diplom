using System.Collections;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ISubjectAssignmentService
{
    Task<ICollection<SubjectAssignment>> GetAllAsync();
    
    Task<SubjectAssignment> AssignSubject(SubjectAssignment dto);

    Task UnassignSubject(int id);

    Task<ICollection<SubjectAssignment>> GetAssignmentsByTeacher(int teacherId);

    Task<ICollection<SubjectAssignment>> GetAssignmentsByGroup(int groupId);
}