using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using System.Collections.ObjectModel;
using System.Windows;

namespace ElectronicJournal.WPF.Views
{
    public partial class DisciplineWin
    {
        private readonly ISubjectAssignmentService _subjectAssignmentService;
        private readonly ITeacherService _teacherService;
        private readonly IGroupService _groupService;
        private readonly ISubjectService _subjectService;

        private ObservableCollection<SubjectAssignment> _assignments;

        public DisciplineWin(
            ISubjectAssignmentService subjectAssignmentService,
            ITeacherService teacherService,
            IGroupService groupService,
            ISubjectService subjectService)
        {
            InitializeComponent();

            _subjectAssignmentService = subjectAssignmentService;
            _teacherService = teacherService;
            _groupService = groupService;
            _subjectService = subjectService;

            Loaded += async (_, _) => await InitializeData();
        }

        private async Task InitializeData()
        {
            try
            {
                await LoadGroups();
                await LoadSubjects();
                await LoadTeachers();
                await ReloadAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }

        private async Task ReloadAssignments()
        {
            var list = await _subjectAssignmentService.GetAllAsync();
            _assignments = new ObservableCollection<SubjectAssignment>(list);
            AssignmentsDataGrid.ItemsSource = _assignments
                .OrderBy(s => s.Teacher?.LastName ?? "")
                .ToList();
        }

        private async Task LoadTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachers();
                TeacherComboBox.ItemsSource = teachers
                    .Select(t => new
                    {
                        t.Id,
                        ShortName = $"{t.LastName} {t.FirstName[0]}. {t.MiddleName?[0]}.".TrimEnd('.')
                    })
                    .OrderBy(t => t.ShortName)
                    .ToList();

                TeacherComboBox.DisplayMemberPath = "ShortName";
                TeacherComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки преподавателей: {ex.Message}");
            }
        }

        private async Task LoadSubjects()
        {
            try
            {
                var subject = await _subjectService.GetAllSubjects();

                SubjectComboBox.ItemsSource = subject.OrderBy(s => s.Name).ToList();
                SubjectComboBox.DisplayMemberPath = "Name";
                SubjectComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки предметов: {ex.Message}");
            }
        }

        private async Task LoadGroups()
        {
            try
            {
                var groups = await _groupService.GetAllAsync();

                GroupComboBox.ItemsSource = groups.OrderBy(g => g.Name).ToList();
                GroupComboBox.DisplayMemberPath = "Name";
                GroupComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}");
            }
        }

        private async void AddBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TeacherComboBox.SelectedValue is int teacherId &&
                    GroupComboBox.SelectedValue is int groupId &&
                    SubjectComboBox.SelectedValue is int subjectId)
                {
                    var newAssignment = new SubjectAssignment
                    {
                        TeacherId = teacherId,
                        GroupId = groupId,
                        SubjectId = subjectId,
                    };

                    var created = await _subjectAssignmentService.AssignSubject(newAssignment);
                    _assignments.Add(created);
                }

                await ReloadAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}");
            }
        }

        private async void EditBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AssignmentsDataGrid.SelectedItem is SubjectAssignment selected &&
                    TeacherComboBox.SelectedValue is int teacherId &&
                    GroupComboBox.SelectedValue is int groupId &&
                    SubjectComboBox.SelectedValue is int subjectId)
                {
                    selected.TeacherId = teacherId;
                    selected.GroupId = groupId;
                    selected.SubjectId = subjectId;

                    await _subjectAssignmentService.AssignSubject(selected);
                    AssignmentsDataGrid.Items.Refresh();
                }

                await ReloadAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования: {ex.Message}");
            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AssignmentsDataGrid.SelectedItem is SubjectAssignment selected)
                {
                    await _subjectAssignmentService.UnassignSubject(selected.Id);
                    _assignments.Remove(selected);
                }

                await ReloadAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}
