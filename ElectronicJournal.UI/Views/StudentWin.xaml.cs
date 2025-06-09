using System.Collections;
using System.Windows;
using ElectronicJournal.Application.Interfaces;

namespace ElectronicJournal.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для StudentWin.xaml
    /// </summary>
    public partial class StudentWin : Window
    {
        private readonly IStudentService _studentService;
        private readonly IAssessmentService _assessmentService;

        public StudentWin(int studentId, IAssessmentService assessmentService, IStudentService studentService)
        {
            InitializeComponent();
            _assessmentService = assessmentService;
            _studentService = studentService;

            LoadStudentInfo(studentId);
            LoadGrades(studentId);
        }

        private async Task LoadStudentInfo(int studentId)
        {
            var student = await _studentService.GetStudentById(studentId);

            if (student == null)
            {
                MessageBox.Show("Ошибка: студент не найден.");
                Close();
                return;
            }

            var fullName = $"{student.LastName} {student.FirstName} {student.MiddleName}";
            var groupName = student.Group?.Name ?? "—";

            namelb.Content = $"{fullName} ({groupName})";
        }

        private void LoadGrades(int studentId)
        {
            var grades = _assessmentService.GetGradesByStudent(studentId);
            dataGrid.ItemsSource = (IEnumerable)grades;
            dataGrid.IsReadOnly = true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}