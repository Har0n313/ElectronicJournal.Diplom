using System.ComponentModel.DataAnnotations;

namespace ElectronicJournal.Domain.Entites;

public class Teacher
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string LastName { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string MiddleName { get; set; }

    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    public ICollection<Subject> Subjects { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}