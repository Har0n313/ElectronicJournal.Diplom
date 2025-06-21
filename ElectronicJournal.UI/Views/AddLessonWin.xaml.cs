using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views;

public partial class AddLessonWin : Window
{
    private readonly int _subjectAssignmentId;
    private readonly ILessonService _lessonService;

    public AddLessonWin(int subjectAssignmentId, ILessonService lessonService)
    {
        InitializeComponent();
        _subjectAssignmentId = subjectAssignmentId;
        _lessonService = lessonService;
    }

    private async void SaveLesson_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var date = (LessonDatePicker.SelectedDate ?? DateTime.Now).ToUniversalTime();
            var type = LessonTypeTextBox.Text;
            var topic = TopicTextBox.Text;

            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(topic))
            {
                MessageBox.Show("Введите тему и тип занятия.");
                return;
            }

            var lesson = new Lesson
            {
                Date = date,
                LessonType = type,
                Topic = topic,
                SubjectAssignmentId = _subjectAssignmentId
            };

            await _lessonService.CreateLesson(lesson);

            MessageBox.Show("Урок успешно добавлен.");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}