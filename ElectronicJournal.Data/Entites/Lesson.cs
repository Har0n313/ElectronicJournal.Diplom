namespace ElectronicJournal.Domain.Entites;

public class Lesson
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string LessonType { get; set; }
    public string Topic { get; set; }

    public int SubjectAssignmentId { get; set; }
    public SubjectAssignment SubjectAssignment { get; set; }

    public ICollection<Grade> Grades { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; }
    public ICollection<Homework> Homeworks { get; set; }
}