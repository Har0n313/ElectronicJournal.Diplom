using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views;

/// <summary>
/// Логика взаимодействия для StudentWin.xaml
/// </summary>
public partial class StudentWin : Window
{
    private readonly IStudentService _studentService;
    private readonly IAssessmentService _assessmentService;
    private readonly IAttendanceService _attendanceService;
    private readonly IScheduleService _scheduleService;
    private readonly int _studentId;
    private int _groupId;

    public StudentWin(int studentId, IAssessmentService assessmentService, IStudentService studentService,
        IScheduleService scheduleService, IAttendanceService attendanceService)
    {
        InitializeComponent();
        _assessmentService = assessmentService;
        _studentService = studentService;
        _scheduleService = scheduleService;
        _attendanceService = attendanceService;
        _studentId = studentId;
        GetGroupId();
    }

    private async void GetGroupId()
    {
        var student = await _studentService.GetStudentById(_studentId);
        _groupId = student.GroupId;
        ListSheduleBt_Click();
    }


    private async void ListSheduleBt_Click()
    {
        await LoadScheduleForDay(DayOfWeek.Monday, mondayGrid);
        await LoadScheduleForDay(DayOfWeek.Tuesday, tuesdayGrid);
        await LoadScheduleForDay(DayOfWeek.Wednesday, wednesdayGrid);
        await LoadScheduleForDay(DayOfWeek.Thursday, thursdayGrid);
        await LoadScheduleForDay(DayOfWeek.Friday, fridayGrid);
        await LoadScheduleForDay(DayOfWeek.Saturday, saturdayGrid);
    }

    private async Task LoadScheduleForDay(DayOfWeek day, DataGrid grid)
    {
        var data = await _scheduleService.GetScheduleByDay(_groupId, day);
        var formatted = data.OrderBy(s => s.PairNumber).Select(s => new
        {
            s.Id,
            s.PairNumber,
            Time = $"{s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm}",
            Subject = s.SubjectAssignment?.Subject?.Name ?? "",
            Teacher = FormatTeacherName(s.SubjectAssignment?.Teacher),
            s.Room
        });
        grid.ItemsSource = formatted.ToList();
    }

    private string FormatTeacherName(Teacher teacher)
    {
        if (teacher == null) return "";
        return $"{teacher.LastName} {teacher.FirstName[0]}. {teacher.MiddleName?[0]}.".TrimEnd('.');
    }

    private async Task LoadGrades(int studentId)
    {
        var assessments = await _assessmentService.GetGradesByStudent(studentId);

        var groupedGrades = assessments
            .GroupBy(a => a.Lesson.SubjectAssignment.Subject.Name)
            .ToDictionary(
                g => g.Key,
                g => g.Select(a => a.MarkValue.ToString() ?? "H").ToList()
            );

        if (!groupedGrades.Any())
        {
            gradesDataGrid.ItemsSource = null;
            return;
        }

        int maxGradesCount = groupedGrades.Values.Max(v => v.Count);

        foreach (var kvp in groupedGrades)
        {
            var grades = kvp.Value;
            while (grades.Count < maxGradesCount)
            {
                grades.Add("—");
            }
        }

        var displayItems = groupedGrades.Select(kvp => new StudentGradeViewModel
        {
            Subject = kvp.Key,
           // Grades = kvp.Value
        }).ToList();

        gradesDataGrid.Columns.Clear();

        gradesDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Предмет",
            Binding = new Binding("Subject"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });

        for (int i = 0; i < maxGradesCount; i++)
        {
            int lessonNumber = i + 1;
            gradesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = $"Урок {lessonNumber}",
                Binding = new Binding($"Grades[{i}]"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }

        gradesDataGrid.ItemsSource = displayItems;
    }
    private async Task LoadAttendanceData(int studentId)
    {
        var attendances = await _attendanceService.GetAttendanceByStudent(studentId);

        var viewModel = attendances.Select(a => new AttendanceViewModel
        {
            Date = a.Date,
            Subject = a.Lesson.SubjectAssignment.Subject.Name,
            Status = GetDisplayName(a.Type)
        }).ToList();

        attendanceDataGrid.ItemsSource = viewModel;
    }

// Помощник для получения отображаемого имени из DisplayAttribute
    private string GetDisplayName(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(
            field,
            typeof(DisplayAttribute)) ?? new DisplayAttribute();

        return attribute.GetName() ?? value.ToString();
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadAttendanceData(_studentId);
    }
}
public class AttendanceViewModel
{
    public string Subject { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } // Например: "Пропущено", "Присутствовал"
}
public class StudentGradeViewModel
{
    public string Subject { get; set; }
    public List<Assessment> Grades { get; set; } // ключ - номер урока или дата, значение - оценка
}