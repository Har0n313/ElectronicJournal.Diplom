using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface ILessonService
{
    Task<Lesson> CreateLesson(Lesson dto);

    Task<Lesson> UpdateLesson(int id, Lesson dto);

    Task<bool> DeleteLesson(int id);

    Task<ICollection<Lesson>> GetLessonsBySubjectAssignment(int subjectAssignmentId);

    Task<ICollection<Lesson>> GetLessonsByGroupAndDate(int groupId, DateTime date);

    Task<ICollection<Lesson>> GetLessonsByTeacherAndDate(int teacherId, DateTime date);
}