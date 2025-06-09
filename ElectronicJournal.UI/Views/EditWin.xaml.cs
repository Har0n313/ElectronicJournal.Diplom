using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views
{
    public partial class EditWin
    {
        private readonly IAssessmentService _assessmentService;
        private int IdDisc { get; set; }
        private int IdTeacher { get; set; }
        private string Klass { get; set; }

        public EditWin(int? idDisc, int idTeach, string cl, IAssessmentService assessmentService)
        {
            InitializeComponent();
            IdDisc = idDisc ?? 0;
            IdTeacher = idTeach;
            Klass = cl;
            _assessmentService = assessmentService;
            LoadAssessmentTypes();
            Update();
        }

        private async void Update()
        {
            var assessments = await _assessmentService.GetAssessmentsByClassAndDiscipline(Klass, IdDisc);
            dataGrid.ItemsSource = assessments;
        }

        private async void EditBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = dataGrid.SelectedItem as Assessment;
                if (selected == null) return;
                
                selected.GradeValue = int.Parse(markTxt.Text);
                selected.Type = GetSelectedAssessmentType();
                selected.DateCreated = datePicker.SelectedDate ?? DateTime.Now;

                await _assessmentService.UpdateAssessments(selected);
                Update();
            }
            catch
            {
                MessageBox.Show("Не все данные были введены корректно");
            }
        }

        private void BackBt_Click(object sender, RoutedEventArgs e)
        {
            var teacherWin = new TeacherWin(IdTeacher, IdDisc, );
            Close();
            teacherWin.Show();
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var selected = dataGrid.SelectedItem as Assessment;
                if (selected == null) return;

                markTxt.Text = selected.GradeValue.ToString();
                datePicker.SelectedDate = selected.DateCreated;
                typeComboBox.SelectedValue = selected.Type;
            }
            catch
            {
                MessageBox.Show("Выберите корректную запись");
            }
        }

        private async void AddBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = dataGrid.SelectedItem as Assessment;
                if (selected == null) return;


                var newAssessment = new Assessment
                {
                    StudentId = selected.StudentId,
                    LessonId = IdDisc,
                    GradeValue = int.Parse(markTxt.Text),
                    DateCreated = GetSelectedDate(),
                    Type = GetSelectedAssessmentType(),
                };

                await _assessmentService.CreateGrade(newAssessment);
                Update();
            }
            catch
            {
                MessageBox.Show("Не удалось добавить запись. Проверьте введённые данные.");
            }
        }

        private void LoadAssessmentTypes()
        {
            var enumValues = Enum.GetValues(typeof(AssessmentType)).Cast<AssessmentType>();
            var displayList = enumValues
                .ToDictionary(
                    val => val,
                    val => val.GetType()
                        .GetMember(val.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()?.Name ?? val.ToString()
                );

            typeComboBox.ItemsSource = displayList;
        }

        // Пример: чтобы получить выбранный тип
        private AssessmentType GetSelectedAssessmentType()
        {
            if (typeComboBox.SelectedValue is AssessmentType selectedType)
                return selectedType;
            MessageBox.Show("Выберите тип оценки");
            return AssessmentType.Homework;
        }

        // Пример: чтобы получить дату
        private DateTime GetSelectedDate()
        {
            return (DateTime)datePicker.SelectedDate!;
        }
    }
}
