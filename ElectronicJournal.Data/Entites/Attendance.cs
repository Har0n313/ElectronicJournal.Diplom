﻿using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.Domain.Entites;

public class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public AbsenceType Type { get; set; }
    public DateTime Date { get; set; }
    public int LessonId { get; set; }
    
    
    public Student Student { get; set; }
    public Lesson Lesson { get; set; }
}