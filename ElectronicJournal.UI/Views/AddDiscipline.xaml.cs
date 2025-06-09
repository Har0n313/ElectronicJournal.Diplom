using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AddDiscipline.xaml
    /// </summary>
    public partial class AddDiscipline
    {
        private readonly ISubjectService _subjectService;
        private Subject? _selectedSubject;

        public AddDiscipline(ISubjectService subjectService)
        {
            _subjectService = subjectService;
            InitializeComponent();
            LoadDisciplines();
        }

        private async void LoadDisciplines()
        {
            try
            {
                DataGrid.ItemsSource = await _subjectService.GetAllSubjects();
            }
            catch (Exception e)
            {
                // ReSharper disable once AsyncVoidMethod
                throw new Exception("Данных не существует");    
            }
        }

        private async void AddBt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DiscTxt.Text) || string.IsNullOrWhiteSpace(CodeTxt.Text))
            {
                MessageBox.Show("Введите все данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var discipline = new Subject
                {
                    Name = DiscTxt.Text,
                    Code = CodeTxt.Text
                };

                await _subjectService.CreateSubject(discipline);
                LoadDisciplines();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditBt_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Выберите дисциплину для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedSubject.Name = DiscTxt.Text;
                _selectedSubject.Code = CodeTxt.Text;

                await _subjectService.UpdateSubject(_selectedSubject);
                LoadDisciplines();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Выберите дисциплину для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show("Вы уверены, что хотите удалить дисциплину?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _subjectService.DeleteSubject(_selectedSubject.Id);
                LoadDisciplines();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _selectedSubject = DataGrid.SelectedItem as Subject;
            if (_selectedSubject != null)
            {
                DiscTxt.Text = _selectedSubject.Name;
                CodeTxt.Text = _selectedSubject.Code;
            }
        }

        private void ClearFields()
        {
            DiscTxt.Text = string.Empty;
            CodeTxt.Text = string.Empty;
            _selectedSubject = null;
        }
    }
}
