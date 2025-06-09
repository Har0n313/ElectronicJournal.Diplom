namespace ElectronicJournal.Domain.Entites;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameSpecialty { get; set; }
    public string SpecialtyCode { get; set; }
    public int AdmissionYear { get; set; }

    public ICollection<Student> Students { get; set; }
    public ICollection<SubjectAssignment> SubjectAssignments { get; set; }
}