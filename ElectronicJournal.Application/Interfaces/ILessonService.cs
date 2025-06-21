using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ILessonService
{
    Task<Lesson> CreateLesson(Lesson dto);

    Task<Lesson> UpdateLesson(Lesson dto);

    Task<bool> DeleteLesson(int id);
    
    Task<Lesson> GetLessonById(int id);

    Task<ICollection<Lesson>> GetLessonsBySubjectAssignment(int subjectAssignmentId);

    Task<Lesson?> GetLessonsByGroupSubjectAndDate(int groupId, int subjectId, DateTime date);

    Task<ICollection<Lesson>> GetLessonsByTeacherAndDate(int teacherId, DateTime date);

    Task<IEnumerable<Lesson>> GetLessonsBySubjectAndGroup(int groupId, int subjectId);
}