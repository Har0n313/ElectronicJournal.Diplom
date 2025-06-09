using System.ComponentModel.DataAnnotations;

namespace ElectronicJournal.Domain.Enums;

public enum AbsenceType
{
    [Display(Name = "По уважительной причине")]
    ValidReason,

    [Display(Name = "По неуважительной причине")]
    InvalidReason,

    [Display(Name = "Пропуск по болезни")]
    SickLeave,

    [Display(Name = "Освобождение от занятий")]
    Excused,

    [Display(Name = "Опоздание")]
    Late
}