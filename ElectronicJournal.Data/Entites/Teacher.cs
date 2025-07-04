﻿namespace ElectronicJournal.Domain.Entites;

public class Teacher
{
    public int Id { get; set; }

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<SubjectAssignment> SubjectAssignments { get; set; }
}
