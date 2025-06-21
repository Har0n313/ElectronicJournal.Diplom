using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views;

public partial class TeacherWin
{
    private readonly IAssessmentService _assessmentService;
    private readonly ISubjectService _subjectService;
    private readonly IGroupService _groupService;
    private readonly IStudentService _studentService;
    private readonly IAttendanceService _attendanceService;
    private readonly IHomeworkService _homeworkService;
    private readonly ISubjectAssignmentService _subjectAssignmentService;
    private readonly ILessonService _lessonService;

    private readonly int _idTeacher;
    private ICollection<Group> _groups;
    private ICollection<Subject> _subjects;
    private ICollection<SubjectAssignment> _assignments;
    private GradeRowViewModel? _selectedRow;
    private DateTime? _selectedLessonDate;

    private readonly Dictionary<string, int> _lessonHeaderMap = new();
    private List<Lesson> _lessons = [];

    public TeacherWin(
        int idTeacher,
        ITeacherService teacherService,
        IAssessmentService assessmentService,
        ISubjectService subjectService,
        IGroupService groupService,
        IStudentService studentService,
        IAttendanceService attendanceService,
        IHomeworkService homeworkService,
        ISubjectAssignmentService subjectAssignmentService,
        ILessonService lessonService)
    {
        InitializeComponent();
        _assessmentService = assessmentService;
        _subjectService = subjectService;
        _groupService = groupService;
        _studentService = studentService;
        _attendanceService = attendanceService;
        _homeworkService = homeworkService;
        _subjectAssignmentService = subjectAssignmentService;
        _lessonService = lessonService;
        _idTeacher = idTeacher;
        Loaded += OnLoaded;
        GradesDataGrid.SelectedCellsChanged += async (sender, e) => await GradesDataGrid_SelectedCellsChanged(sender, e);
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await LoadData();
            _assignments = (await _subjectAssignmentService.GetAssignmentsByTeacher(_idTeacher)).ToList();
            UpdateComboBoxes();
            GroupComboBox.SelectionChanged += (_, _) => UpdateComboBoxes();
            SubjectComboBox.SelectionChanged += (_, _) => UpdateComboBoxes();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task LoadData()
    {
        try
        {
            var assignments = await _subjectAssignmentService.GetAssignmentsByTeacher(_idTeacher);
            _groups = assignments
                .Select(a => a.Group)
                .DistinctBy(g => g.Id)
                .ToList();
            _subjects = assignments
                .Select(a => a.Subject)
                .DistinctBy(s => s.Id)
                .ToList();

            GroupComboBox.ItemsSource = _groups.OrderBy(g => g.Name).ToList();
            GroupComboBox.DisplayMemberPath = "Name";
            GroupComboBox.SelectedValuePath = "Id";

            SubjectComboBox.ItemsSource = _subjects.OrderBy(s => s.Name).ToList();
            SubjectComboBox.DisplayMemberPath = "Name";
            SubjectComboBox.SelectedValuePath = "Id";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
        }
    }

    private void LoadAttendance_Click(object sender, RoutedEventArgs e)
    {
        if (GroupComboBox.SelectedValue is int groupId && SubjectComboBox.SelectedValue is int subjectId)
        {
            // Пример - нужно реализовать PassWin
            // var passWin = new PassWin(groupId, subjectId, _attendanceService, _studentService);
            // passWin.Owner = this;
            // passWin.ShowDialog();
        }
        else
        {
            MessageBox.Show("Выберите группу и предмет.");
        }
    }

    private async void SaveHomework_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (GroupComboBox.SelectedValue is int groupId &&
                SubjectComboBox.SelectedValue is int subjectId &&
                !string.IsNullOrWhiteSpace(HomeworkTextBox.Text))
            {
                try
                {
                    var lesson = _lessonService.GetLessonsByGroupSubjectAndDate(groupId, subjectId,
                        HomeworkDatePicker.SelectedDate.Value);
                    await _homeworkService.CreateHomework(new Homework
                    {
                        Description = HomeworkTextBox.Text,
                        DueDate = HomeworkDatePicker.SelectedDate ?? DateTime.Now,
                        LessonId = lesson.Id,
                        TeacherId = _idTeacher
                    });
                    MessageBox.Show("Домашнее задание сохранено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void UpdateComboBoxes()
    {
        var selectedGroupId = (int?)GroupComboBox.SelectedValue;
        var selectedSubjectId = (int?)SubjectComboBox.SelectedValue;
        var filteredAssignments = _assignments.AsEnumerable();

        if (selectedSubjectId is int subjectId)
            filteredAssignments = filteredAssignments.Where(a => a.SubjectId == subjectId);

        if (selectedGroupId is int groupId)
            filteredAssignments = filteredAssignments.Where(a => a.GroupId == groupId);

        var filteredGroups = filteredAssignments.Select(a => a.Group).DistinctBy(g => g.Id).OrderBy(g => g.Name).ToList();
        var filteredSubjects = filteredAssignments.Select(a => a.Subject).DistinctBy(s => s.Id).OrderBy(s => s.Name).ToList();

        GroupComboBox.ItemsSource = filteredGroups.DistinctBy(g => g.Id).OrderBy(g => g.Name).ToList();
        SubjectComboBox.ItemsSource = filteredSubjects.DistinctBy(s => s.Id).OrderBy(s => s.Name).ToList();

        GroupComboBox.SelectedValue = selectedGroupId;
        SubjectComboBox.SelectedValue = selectedSubjectId;
    }

    private void ClearComboBoxes()
    {
        GroupComboBox.ItemsSource = null;
        GroupComboBox.SelectedItem = null;
        GroupComboBox.SelectedValue = null;

        SubjectComboBox.ItemsSource = null;
        SubjectComboBox.SelectedItem = null;
        SubjectComboBox.SelectedValue = null;
    }

    private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        ClearComboBoxes();
        UpdateComboBoxes(); // загрузит все группы и предметы заново
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void RefreshGrades_Click(object sender, RoutedEventArgs e)
    {
        if (GroupComboBox.SelectedValue is int groupId && SubjectComboBox.SelectedValue is int subjectId)
        {
            await LoadGradesAsync(groupId, subjectId);
        }
        else
        {
            MessageBox.Show("Выберите и группу, и предмет для отображения оценок.");
        }
    }

    private async Task LoadGradesAsync(int groupId, int subjectId)
    {
        var students = await _groupService.GetGroupStudents(groupId);
        var assignment = _assignments.FirstOrDefault(a => a.GroupId == groupId && a.SubjectId == subjectId);
        if (assignment == null)
        {
            MessageBox.Show("Нет назначенных занятий по выбранным параметрам.");
            return;
        }

        _lessons = (await _lessonService.GetLessonsBySubjectAssignment(assignment.Id)).ToList();

        var allGrades = new List<Assessment>();
        foreach (var lesson in _lessons)
        {
            var grades = await _assessmentService.GetAssessmentByLesson(lesson.Id);
            allGrades.AddRange(grades);
        }

        var table = new List<GradeRowViewModel>();
        foreach (var student in students)
        {
            var row = new GradeRowViewModel
            {
                FullName = $"{student.LastName} {student.FirstName}"
            };
            foreach (var lesson in _lessons)
            {
                var grade = allGrades.FirstOrDefault(a => a.StudentId == student.Id && a.LessonId == lesson.Id);
                string markDisplay = grade != null
                    ? (grade.Type == AssessmentType.Homework ? $"Д{grade.MarkValue}" : grade.MarkValue.ToString())
                    : "";
                row.GradesByLessonId[lesson.Id] = markDisplay;
            }
            table.Add(row);
        }

        BuildDynamicDataGrid(table);
    }

    private void BuildDynamicDataGrid(List<GradeRowViewModel> rows)
    {
        GradesDataGrid.Columns.Clear();
        GradesDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ФИО",
            Binding = new Binding("FullName"),
            IsReadOnly = true
        });

        _lessonHeaderMap.Clear();

        foreach (var lesson in _lessons.OrderBy(l => l.Date))
        {
            string header = lesson.Date.ToString("dd MMM", new CultureInfo("ru-RU"));
            _lessonHeaderMap[header] = lesson.Id;

            GradesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding($"GradesByLessonId[{lesson.Id}]"),
                IsReadOnly = true
            });
        }

        GradesDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Средняя",
            Binding = new Binding("Average") { StringFormat = "F2" },
            IsReadOnly = true
        });

        GradesDataGrid.ItemsSource = rows;
    }

    private async void AddLessonButton_Click(object sender, RoutedEventArgs e)
    {
        if (GroupComboBox.SelectedValue is int groupId && SubjectComboBox.SelectedValue is int subjectId)
        {
            var assignment = _assignments.FirstOrDefault(a => a.GroupId == groupId && a.SubjectId == subjectId);
            if (assignment == null)
            {
                MessageBox.Show("Не найдено назначение для выбранной группы и предмета.");
                return;
            }

            var addLessonWin = new AddLessonWin(assignment.Id, _lessonService);
            addLessonWin.Owner = this;
            addLessonWin.ShowDialog();
        }
        else
        {
            MessageBox.Show("Выберите и группу, и предмет перед добавлением урока.");
        }
    }

    private async void AddGradeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRow == null || _selectedLessonDate == null)
        {
            MessageBox.Show("Выберите ячейку с оценкой.");
            return;
        }

        var assignment = _assignments.FirstOrDefault(a =>
            a.GroupId == (int)GroupComboBox.SelectedValue &&
            a.SubjectId == (int)SubjectComboBox.SelectedValue);

        if (assignment == null)
        {
            MessageBox.Show("Назначение не найдено.");
            return;
        }

        var student = (await _groupService.GetGroupStudents((int)GroupComboBox.SelectedValue))
            .FirstOrDefault(s => $"{s.LastName} {s.FirstName}" == _selectedRow.FullName);

        if (student == null)
        {
            MessageBox.Show("Студент не найден.");
            return;
        }

        var selectedLesson = _lessons.FirstOrDefault(l => l.Date.Date == _selectedLessonDate.Value.Date);
        if (selectedLesson == null)
        {
            MessageBox.Show("Урок по выбранной дате не найден.");
            return;
        }

        var addGradeWin = new AddGradeWin(student.Id, selectedLesson.Id, _assessmentService);
        addGradeWin.Owner = this;
        addGradeWin.ShowDialog();

        await LoadGradesAsync((int)GroupComboBox.SelectedValue, (int)SubjectComboBox.SelectedValue);
    }

    private async Task GradesDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        try
        {
            if (GradesDataGrid.CurrentCell.Column is DataGridTextColumn column &&
                GradesDataGrid.CurrentItem is GradeRowViewModel row)
            {
                _selectedRow = row;

                if (column.Header is string header && _lessonHeaderMap.TryGetValue(header, out var lessonId))
                {
                    var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
                    _selectedLessonDate = lesson?.Date;
                }
                else
                {
                    _selectedLessonDate = null;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при выборе ячейки: {ex.Message}");
        }
    }
}

public class GradeRowViewModel
{
    public string FullName { get; set; }
    public Dictionary<int, string> GradesByLessonId { get; set; } = new();
    public double Average => GradesByLessonId.Values
        .Where(v => int.TryParse(v, out _))
        .Select(v => int.Parse(v))
        .DefaultIfEmpty(0)
        .Average();
}