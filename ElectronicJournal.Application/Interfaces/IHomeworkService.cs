using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.Application.Interfaces;

public interface IHomeworkService
{
    Task<Homework> CreateHomework(Homework dto);

    Task<Homework> UpdateHomework(int id, Homework dto);

    Task<bool> DeleteHomework(int id);

    Task<ICollection<Homework>> GetHomeworkByLesson(int lessonId);

    Task<ICollection<Homework>> GetHomeworkByGroup(int groupId);

    Task<ICollection<Homework>> GetHomeworkByStudent(int studentId);
}