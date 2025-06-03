namespace ElectronicJournal.Domain.Entites;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }

    public ICollection<SubjectAssignment> SubjectAssignments { get; set; }
}