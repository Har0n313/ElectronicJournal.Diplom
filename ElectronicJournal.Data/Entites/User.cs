using System.ComponentModel.DataAnnotations;

namespace ElectronicJournal.Domain.Entites;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; }

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required, MaxLength(20)]
    public string Role { get; set; } // student, teacher, admin

    public int? LinkedId { get; set; } // StudentId или TeacherId в зависимости от роли
}