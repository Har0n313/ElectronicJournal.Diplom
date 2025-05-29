using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicJournal.Domain.Entites;

public class Subject
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [ForeignKey(nameof(Teacher))]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public ICollection<Lesson> Lessons { get; set; }
}