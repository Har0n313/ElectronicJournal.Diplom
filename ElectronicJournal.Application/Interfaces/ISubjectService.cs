using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ISubjectService
{
    Task<ICollection<Subject>> GetAllSubjects();

    Task<Subject> GetSubjectById(int id);

    Task<Subject> CreateSubject(Subject subject);

    Task<Subject> UpdateSubject(Subject subject);
    
    Task<ICollection<Subject>> GetSubjectsByTeacher(int id);

    Task<bool> DeleteSubject(int id);
}