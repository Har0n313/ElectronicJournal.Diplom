namespace ElectronicJournal.Application.dto;

public class AssessmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int GradeValue { get; set; }
    public DateTime DateCreated { get; set; }
}