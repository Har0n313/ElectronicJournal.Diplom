using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.Domain.Entites;

public class Assessment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int MarkValue { get; set; } 
    public AssessmentType Type { get; set; }
    

    
    public Student Student { get; set; }
    public Lesson Lesson { get; set; }
}