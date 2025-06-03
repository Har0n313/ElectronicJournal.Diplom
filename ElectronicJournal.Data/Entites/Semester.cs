namespace ElectronicJournal.Domain.Entites;

public class Semester
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<SubjectAssignment> SubjectAssignments { get; set; }
}