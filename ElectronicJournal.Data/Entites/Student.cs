namespace ElectronicJournal.Domain.Entites;

public class Student
{
    public int Id { get; set; }

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }


    public int GroupId { get; set; }
    public Group Group { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<Assessment> Grades { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; }
}
