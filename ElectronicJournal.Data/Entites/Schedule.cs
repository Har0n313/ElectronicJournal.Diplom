namespace ElectronicJournal.Domain.Entites;

public class Schedule
{
    public int Id { get; set; }

    public DayOfWeek Day { get; set; } 
    public int PairNumber { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Room { get; set; }

    public int SubjectAssignmentId { get; set; }
    public SubjectAssignment SubjectAssignment { get; set; }
}