using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicJournal.Domain.Entites;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string LastName { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string MiddleName { get; set; }

    [ForeignKey(nameof(Group))]
    public int GroupId { get; set; }
    public Group Group { get; set; }

    public DateTime DateOfBirth { get; set; }

    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    public ICollection<Grade> Grades { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; }
}