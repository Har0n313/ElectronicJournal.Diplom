using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для TeacherWin.xaml
    /// </summary>
    public partial class TeacherWin : Window
    {
        public int IdTeacher { get; set; }
        private int IdDisc;
        string surname, name, middleName;
        private readonly ITeacherService _teacherService;
        private readonly IAssessmentService _assessmentService;
        private readonly ISubjectService _subjectService;
        private readonly IGroupService _groupService;
        private readonly IStudentService _studentService;
        private readonly IAttendanceService _attendanceService;
        private readonly IHomeworkService _homeworkService;

        public TeacherWin(
            int idTeacher,
            ITeacherService teacherService,
            IAssessmentService assessmentService,
            ISubjectService subjectService,
            IGroupService groupService,
            IStudentService studentService,
            IAttendanceService attendanceService,
            IHomeworkService homeworkService)
        {
            InitializeComponent();

            _teacherService = teacherService;
            _assessmentService = assessmentService;
            _subjectService = subjectService;
            _groupService = groupService;
            _studentService = studentService;
            _attendanceService = attendanceService;
            _homeworkService = homeworkService;

            IdTeacher = idTeacher;

            Loaded += TeacherWin_Loaded;
        }

        private async void TeacherWin_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeData();
            await TeacherData(IdTeacher);
        }

        private async Task InitializeData()
        {
            var classes = await _groupService.GetAllGroups();
            classComboBox.ItemsSource = classes;
            attendanceClassComboBox.ItemsSource = classes;
            homeworkClassComboBox.ItemsSource = classes;

            var subjects = await _subjectService.GetSubjectsByTeacher(IdTeacher);
            subjectComboBox.ItemsSource = subjects;
            homeworkSubjectComboBox.ItemsSource = subjects;
        }

        private async Task TeacherData(int idTeacher)
        {
            var teacher = await _teacherService.GetTeacherById(idTeacher);
            surname = teacher.LastName;
            name = teacher.FirstName;
            middleName = teacher.MiddleName;
            var subjects = await _subjectService.GetSubjectsByTeacher(idTeacher);
            var firstSubject = subjects.FirstOrDefault();
            if (firstSubject != null)
                IdDisc = firstSubject.Id;
        }

        private async void RefreshGrades_Click(object sender, RoutedEventArgs e)
        {
            var group = classComboBox.SelectedItem as Group;
            var subject = subjectComboBox.SelectedItem as Subject;
            if (group == null || subject == null)
                return;

            var grades = await _assessmentService.GetAssessmentsByClassAndDiscipline(group.Id, subject.Id);
            gradesDataGrid.ItemsSource = grades;
        }

        private void LoadAttendance_Click(object sender, RoutedEventArgs e)
        {
            var group = attendanceClassComboBox.SelectedItem as Group;
            if (group == null) return;
            PassWin passWin = new PassWin(group.Id, IdDisc, _attendanceService, _studentService);
            passWin.ShowDialog();
        }

        private async void SaveHomework_Click(object sender, RoutedEventArgs e)
        {
            if (homeworkClassComboBox.SelectedItem is Group group && homeworkSubjectComboBox.SelectedItem is Subject subject)
            {
                var date = homeworkDatePicker.SelectedDate ?? System.DateTime.Now;
                var text = homeworkTextBox.Text;
                await _homeworkService.CreateHomework(group.Id, subject.Id, date, text);
                homeworkDataGrid.ItemsSource = await _homeworkService.GetHomeworkByClassAndSubject(group.Id, subject.Id);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void listDiscipline_Click(object sender, RoutedEventArgs e)
        {
            DisciplineWin disciplineWin = new DisciplineWin(IdTeacher, _teacherService);
            disciplineWin.ShowDialog();
        }

        private void editBt_Click(object sender, RoutedEventArgs e)
        {
            EditWin editWin = new EditWin(IdDisc, IdTeacher, classTxtBx.Text);
            Close();
            editWin.Show();
        }
    }
}
