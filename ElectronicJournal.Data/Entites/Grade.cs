using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicJournal.Domain.Entites;

public class Grade
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Student))]
    public int StudentId { get; set; }
    public Student Student { get; set; }

    [ForeignKey(nameof(Lesson))]
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }

    [MaxLength(5)]
    public string Value { get; set; }

    public string Comment { get; set; }
}