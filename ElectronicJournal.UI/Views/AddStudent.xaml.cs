using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;
using ElectronicJournal.Application.Interfaces;

namespace ElectronicJournal.WPF.Views
{
    public partial class AddStudent
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        
        private int _currentGroupId;

        public AddStudent(
            IUserService userService,
            IStudentService studentService,
            IGroupService groupService)
        {
            InitializeComponent(); // <-- Теперь первым!

            _userService = userService;
            _studentService = studentService;
            _groupService = groupService;

            LoadData();

        }

        private async void LoadData()
        {
            try
            {
                var students = await _studentService.GetAllStudents();
                dataGrid.ItemsSource = students.OrderBy(s => s.LastName);
                dataGrid.IsReadOnly = true;            
                LoadGroups();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
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

        private async void AddBt_Click(object sender, RoutedEventArgs e)
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

                var user = new User
                {
                    UserName = loginTxt.Text,
                    PasswordHash = passTxt.Text,
                    Role = UserRole.Student
                };

                await _userService.RegisterUser(user);

                var authUser = await _userService.Authenticate(user.UserName, user.PasswordHash);

                var student = new Student
                {
                    FirstName = nameTxt.Text,
                    LastName = surnameTxt.Text,
                    MiddleName = middleName.Text,
                    UserId = authUser.Id,
                    GroupId = _currentGroupId
                };

                await _studentService.CreateStudent(student);
                MessageBox.Show("Студент успешно добавлен");

                LoadData();               
                Clear();

            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "нет дополнительной информации";
                MessageBox.Show($"Ошибка при добавлении студента:\n{ex.Message}\n\nInner: {innerMessage}");
            }

        }

        private async void EditBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is not Student selectedStudent)
                {
                    MessageBox.Show("Выберите студента");
                    return;
                }

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

                selectedStudent.FirstName = nameTxt.Text;
                selectedStudent.LastName = surnameTxt.Text;
                selectedStudent.MiddleName = middleName.Text;
                selectedStudent.GroupId = _currentGroupId;

                var user = await _userService.GetUserInfo(selectedStudent.UserId);
                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден");
                    return;
                }

                user.UserName = loginTxt.Text;
                user.PasswordHash = passTxt.Text;
                await _userService.UpdateUser(user);

                await _studentService.UpdateStudent(selectedStudent);
                MessageBox.Show("Данные обновлены");

                LoadData();
                Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании студента: {ex.Message}");
                Clear();

            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is not Student selectedStudent)
                {
                    MessageBox.Show("Выберите студента для удаления");
                    return;
                }

                var confirmResult = MessageBox.Show(
                    $"Вы уверены, что хотите удалить студента {selectedStudent.LastName} {selectedStudent.FirstName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes)
                    return;

                // Удаляем студента
                await _studentService.DeleteStudent(selectedStudent.Id);

                // Удаляем пользователя
                await _userService.DeletUser(selectedStudent.UserId);

                MessageBox.Show("Студент успешно удалён");

                LoadData();
                Clear();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "нет дополнительной информации";
                MessageBox.Show($"Ошибка при удалении студента:\n{ex.Message}\n\nInner: {innerMessage}");
            }

        }


        private async void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is not Student selectedStudent) return;

                nameTxt.Text = selectedStudent.FirstName;
                surnameTxt.Text = selectedStudent.LastName;
                middleName.Text = selectedStudent.MiddleName;

                GroupComboBox.SelectedValue = selectedStudent.GroupId;

                var user = await _userService.GetUserInfo(selectedStudent.UserId);
                if (user == null) return;

                loginTxt.Text = user.UserName;
                passTxt.Text = user.PasswordHash;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе студента: {ex.Message}");
            }
        }

        private void Clear()
        {
            nameTxt.Text = string.Empty;
            surnameTxt.Text = string.Empty;
            middleName.Text = string.Empty;
            loginTxt.Text = string.Empty;
            passTxt.Text = string.Empty;
            _currentGroupId = 0;
        }
    }
}