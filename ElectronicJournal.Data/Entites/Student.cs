namespace ElectronicJournal.Domain.Entites;

public class Student
{
    public int Id { get; set; }

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; } // может быть null

    public DateTime BirthDate { get; set; }
    public string RecordBookNo { get; set; }
    public string Status { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }

    public ICollection<Grade> Grades { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; }
}
