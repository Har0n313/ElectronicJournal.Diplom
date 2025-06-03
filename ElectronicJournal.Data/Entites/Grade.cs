namespace ElectronicJournal.Domain.Entites;

public class Grade
{
    public int Id { get; set; }
    public string GradeValue { get; set; } 
    public string Comment { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; }

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
}