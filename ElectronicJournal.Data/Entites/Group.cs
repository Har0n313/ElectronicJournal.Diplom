using System.ComponentModel.DataAnnotations;

namespace ElectronicJournal.Domain.Entites;

public class Group
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; }

    public int Course { get; set; }

    [MaxLength(100)]
    public string Specialty { get; set; }

    public ICollection<Student> Students { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}