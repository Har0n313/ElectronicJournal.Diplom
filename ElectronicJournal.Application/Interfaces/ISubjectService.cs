using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ISubjectService
{
    Task<ICollection<Subject>> GetAllSubjects();

    Task<Subject> GetSubjectById(int id);

    Task<Subject> CreateSubject(Subject subject);

    Task<Subject> UpdateSubject(int id, Subject subject);

    Task<bool> DeleteSubject(int id);
}