using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IGradeService
{
    Task<Grade> CreateGrade(int studentId, int lessonId, Grade dto);

    Task<Grade> UpdateGrade(int id, Grade dto);

    Task<bool> DeleteGrade(int id);

    Task<ICollection<Grade>> GetGradesByStudent(int studentId);

    Task<ICollection<Grade>> GetGradesByLesson(int lessonId);
}