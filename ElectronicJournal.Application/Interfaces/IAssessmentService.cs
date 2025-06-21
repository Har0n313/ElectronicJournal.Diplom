using ElectronicJournal.Application.dto;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IAssessmentService
{
    Task<IEnumerable<AssessmentDto>> GetAssessmentsByClassAndDiscipline(string className, int disciplineId);
    Task<Assessment> CreateAssessment(Assessment assessment);

    Task<Assessment> UpdateAssessments(Assessment assessment);

    Task<bool> DeleteAssessment(int id);

    Task<ICollection<Assessment>> GetGradesByStudent(int studentId);

    Task<ICollection<Assessment>> GetAssessmentByLesson(int lessonId);
}