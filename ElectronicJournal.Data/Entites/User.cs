using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.Domain.Entites;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public int? StudentId { get; set; }
    public Student Student { get; set; }

    public int? TeacherId { get; set; }
    public Teacher Teacher { get; set; }
}