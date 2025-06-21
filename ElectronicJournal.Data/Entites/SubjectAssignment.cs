using System.Collections;

namespace ElectronicJournal.Domain.Entites;

public class SubjectAssignment
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public ICollection<Schedule> Schedules { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}