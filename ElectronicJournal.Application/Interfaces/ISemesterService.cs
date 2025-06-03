using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ISemesterService
{
    Task<ICollection<Semester>> GetAllSemesters();

    Task<Semester> GetSemesterById(int id);

    Task<Semester> CreateSemester(Semester dto);

    Task<Semester> UpdateSemester(int id, Semester dto);

    Task<bool> DeleteSemester(int id);
}