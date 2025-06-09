namespace ElectronicJournal.Domain.Entites;

public class Homework
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public int LessonId { get; set; }
    public int TeacherId { get; set; }
    
    public Teacher Teacher { get; set; }
    public Lesson Lesson { get; set; }
}