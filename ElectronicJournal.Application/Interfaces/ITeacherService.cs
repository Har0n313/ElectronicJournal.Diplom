using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ITeacherService
{
    Task<ICollection<Teacher>> GetAllTeachers();

    Task<Teacher> GetTeacherById(int id);

    Task<Teacher> CreateTeacher(Teacher dto);

    Task<Teacher> UpdateTeacher(int id, Teacher dto);

    Task<bool> DeleteTeacher(int id);

    Task<ICollection<Subject>> GetAssignedSubjects(int teacherId);

    Task<ICollection<Lesson>> GetTeacherSchedule(int teacherId);
}