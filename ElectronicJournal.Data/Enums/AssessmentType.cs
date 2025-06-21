using System.ComponentModel.DataAnnotations;

namespace ElectronicJournal.Domain.Enums;

public enum AssessmentType
{
    [Display(Name = "Домашнее задание")] Homework,

    [Display(Name = "Работа в классе")] Classwork,

    [Display(Name = "Контрольная работа")] ControlWork,

    [Display(Name = "Практическая работа")]
    PracticalWork,

    [Display(Name = "Лабораторная работа")]
    LabWork,

    [Display(Name = "Тест")] Test,

    [Display(Name = "Курсовая работа")] CourseProject,

    [Display(Name = "Зачёт")] Credit,

    [Display(Name = "Дифференцированный зачёт")]
    DiffCredit,

    [Display(Name = "Экзамен")] Exam,

    [Display(Name = "Государственная итоговая аттестация")]
    FinalStateExam
}