using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views
{
    public partial class PassWin
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentService _studentService;

        private int? IdDisc { get; set; }
        private string kls { get; set; }

        public PassWin(string klass, int? idDisc, IAttendanceService attendanceService, IStudentService studentService)
        {
            InitializeComponent();
            IdDisc = idDisc;
            kls = klass;
            _attendanceService = attendanceService;
            _studentService = studentService;

            LoadStudents();
            LoadAbsenceTypes();
        }

        private async void LoadStudents()
        {
            try
            {
                //var students = await _studentService.GetStudentsByClassName(kls);
                //dataGrid.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке студентов: {ex.Message}");
            }
        }

        private void LoadAbsenceTypes()
        {
            var items = Enum.GetValues(typeof(AbsenceType))
                .Cast<AbsenceType>()
                .Select(type => new KeyValuePair<AbsenceType, string>(type, GetDisplayName(type)))
                .ToList();

            absenceTypeComboBox.ItemsSource = items;
        }

        private string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }

        private async void saveBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedStudent = dataGrid.SelectedItem as Student;
                if (selectedStudent == null)
                {
                    MessageBox.Show("Выберите студента.");
                    return;
                }

                if (absenceTypeComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите тип пропуска.");
                    return;
                }

                if (!DateTime.TryParse(dateTxt.Text, out var selectedDate))
                {
                    MessageBox.Show("Неверный формат даты.");
                    return;
                }

                var selectedType = (AbsenceType)absenceTypeComboBox.SelectedValue;

                var attendance = new Attendance
                {
                    StudentId = selectedStudent.Id,
                    LessonId = IdDisc ?? throw new InvalidOperationException("Id дисциплины не указан."),
                    Type = selectedType,
                    Date = selectedDate
                };

                await _attendanceService.MarkAttendance(attendance);
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void listBt_Click(object sender, RoutedEventArgs e)
        {
            //var listPass = new ListPass(IdDisc, _attendanceService);
            //listPass.ShowDialog();
        }
    }
}
