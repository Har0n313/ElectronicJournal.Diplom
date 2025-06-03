using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IStudentService
{
    Task<ICollection<Student>> GetAllStudents();

    Task<Student> GetStudentById(int id);

    Task<Student> CreateStudent(Student dto);

    Task<Student> UpdateStudent(int id, Student student);

    Task<bool> DeleteStudent(int id);

    Task<ICollection<Grade>> GetStudentGrades(int id);

    Task<ICollection<Attendance>> GetStudentAttendance(int id);

    Task<Semester> GetStudentCurrentSemester(int id);
}