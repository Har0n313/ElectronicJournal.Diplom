using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views;

/// <summary>
/// Логика взаимодействия для AdminWin.xaml
/// </summary>
public partial class AdminWin
{
    private readonly IScheduleService _scheduleService;
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IUserService _userService;
    private readonly IStudentService _studentService;
    private readonly ISubjectAssignmentService _subjectAssignmentService;
    private int _currentGroupId;

    public AdminWin(IScheduleService scheduleService, IGroupService groupService,
        ITeacherService teacherService, ISubjectService subjectService, IUserService userService,
        IStudentService studentService, ISubjectAssignmentService subjectAssignmentService)
    {
        _scheduleService = scheduleService;
        _groupService = groupService;
        _teacherService = teacherService;
        _subjectService = subjectService;
        _userService = userService;
        _studentService = studentService;
        _subjectAssignmentService = subjectAssignmentService;
        InitializeComponent();
        LoadGroups();
    }

    private async Task LoadScheduleForDay(DayOfWeek day, DataGrid grid)
    {
        var data = await _scheduleService.GetScheduleByDay(_currentGroupId, day);
        var formatted = data.OrderBy(s => s.PairNumber).Select(s => new
        {
            s.Id,
            s.PairNumber,
            Time = $@"{s.StartTime:hh\:mm} - {s.EndTime:hh\:mm}",
            Subject = s.SubjectAssignment?.Subject?.Name ?? "",
            Teacher = FormatTeacherName(s.SubjectAssignment?.Teacher),
            s.Room
        });
        grid.ItemsSource = formatted.ToList();
    }

    private string FormatTeacherName(Teacher? teacher)
    {
        return teacher == null
            ? ""
            : $"{teacher.LastName} {teacher.FirstName[0]}. {teacher.MiddleName?[0]}.".TrimEnd('.');
    }

    private void AddTeacherBt_Click(object sender, RoutedEventArgs e)
    {
        var addTeacherWin = new AddTeacherWin(_teacherService, _userService);
        addTeacherWin.ShowDialog();
    }

    private void SheduleBt_Click(object sender, RoutedEventArgs e)
    {
        ScheduleWin scheduleWin =
            new ScheduleWin(_scheduleService, _groupService, _teacherService, _subjectService);
        scheduleWin.ShowDialog();
    }

    private async void ListSheduleBt_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (GroupComboBox.SelectedItem is not Group selectedGroup)
            {
                MessageBox.Show("Выберите группу");
                return;
            }

            var group = (await _groupService.GetAllAsync()).FirstOrDefault(g => g.Name == selectedGroup.Name);
            if (group == null)
            {
                MessageBox.Show("Группа не найдена");
                return;
            }

            _currentGroupId = group.Id;

            await LoadScheduleForDay(DayOfWeek.Monday, mondayGrid);
            await LoadScheduleForDay(DayOfWeek.Tuesday, tuesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Wednesday, wednesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Thursday, thursdayGrid);
            await LoadScheduleForDay(DayOfWeek.Friday, fridayGrid);
            await LoadScheduleForDay(DayOfWeek.Saturday, saturdayGrid);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
        }
    }

    private void AddStudentBt_Click(object sender, RoutedEventArgs e)
    {
        AddStudent addStudent = new AddStudent(_userService, _studentService, _groupService);
        addStudent.ShowDialog();
    }

    private void AddGroupBt_Click(object sender, RoutedEventArgs e)
    {
        AddGroup addGroup = new AddGroup(_groupService);
        addGroup.ShowDialog();
    }

    private void AddDiscBt_Click(object sender, RoutedEventArgs e)
    {
        AddDiscipline addDiscipline = new AddDiscipline(_subjectService);
        addDiscipline.ShowDialog();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        Owner?.Show();
        Close();
    }

    private async void LoadGroups()
    {
        try
        {
            var groups = await _groupService.GetAllAsync();

            if (GroupComboBox != null)
            {
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "Name";
                GroupComboBox.SelectedValuePath = "Id";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке групп: {ex.Message}");
        }
    }

    private void AddAssignmentBt_Click(object sender, RoutedEventArgs e)
    {
        var dist = new DisciplineWin(_subjectAssignmentService, _teacherService, _groupService, _subjectService);
        dist.Show();
    }
}