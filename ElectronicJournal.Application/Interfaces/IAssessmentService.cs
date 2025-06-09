using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IAssessmentService
{
    Task<IEnumerable<Assessment>> GetAssessmentsByClassAndDiscipline(string className, int disciplineId);
    Task<Assessment> CreateGrade(Assessment assessment);

    Task<Assessment> UpdateAssessments(Assessment assessment);

    Task<bool> DeleteGrade(int id);

    Task<ICollection<Assessment>> GetGradesByStudent(int studentId);

    Task<ICollection<Assessment>> GetGradesByLesson(int lessonId);
}