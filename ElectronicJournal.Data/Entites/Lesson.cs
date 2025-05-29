using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicJournal.Domain.Entites;

public class Lesson
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Group))]
    public int GroupId { get; set; }
    public Group Group { get; set; }

    [ForeignKey(nameof(Subject))]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    [ForeignKey(nameof(Teacher))]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public DateTime Date { get; set; }

    public int LessonNum { get; set; }

    public ICollection<Grade> Grades { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; }
}