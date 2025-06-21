using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views;

public partial class AddGradeWin : Window
{
    private readonly int _studentId;
    private readonly int _lessonId;
    private readonly IAssessmentService _assessmentService;

    public AddGradeWin(int studentId, int lessonId, IAssessmentService assessmentService)
    {
        InitializeComponent();
        _studentId = studentId;
        _lessonId = lessonId;
        _assessmentService = assessmentService;
        AssessmentTypeComboBox.ItemsSource = Enum.GetValues(typeof(AssessmentType)).Cast<AssessmentType>();
    }

    private async void SaveGrade_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(GradeTextBox.Text, out int gradeValue) &&
            AssessmentTypeComboBox.SelectedItem is AssessmentType type && gradeValue is > 0 and < 6)
        {
            await _assessmentService.CreateAssessment(new Assessment
            {
                StudentId = _studentId,
                LessonId = _lessonId,
                MarkValue = gradeValue,
                Type = type,
            });

            MessageBox.Show("Оценка добавлена.");
            Close();
        }
        else
        {
            MessageBox.Show("Введите корректную оценку и выберите тип.");
        }
    }
}