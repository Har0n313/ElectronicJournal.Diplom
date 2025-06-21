using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IStudentService
{
    Task<ICollection<Student>> GetAllStudents();

    Task<Student> GetStudentById(int id);

    Task<Student> CreateStudent(Student dto);

    Task<Student> UpdateStudent(Student student);

    Task<bool> DeleteStudent(int id);

    Task<ICollection<Assessment>> GetStudentGrades(int id);

    Task<ICollection<Attendance>> GetStudentAttendance(int id);
}