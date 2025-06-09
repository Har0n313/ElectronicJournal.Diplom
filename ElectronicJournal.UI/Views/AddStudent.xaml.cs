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

        public AddStudent(IUserService userService, IStudentService studentService, IGroupService groupService)
        {
            _userService = userService;
            _studentService = studentService;
            _groupService = groupService;

            InitializeComponent();
            LoadData();
            LoadGroups();
        }

        private async void LoadData()
        {
            try
            {
                dataGrid.ItemsSource = await _studentService.GetAllStudents();
                dataGrid.IsReadOnly = true;
            }
            catch (Exception e)
            {
                // ReSharper disable once AsyncVoidMethod
                throw new Exception($"Ошибка при загрузке данных {e}");
            }
        }

        private async void LoadGroups()
        {
            try
            {
                var groups = await _groupService.GetAllGroups();
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "Name"; // Убедись, что у группы есть свойство Name
                GroupComboBox.SelectedValuePath = "Id";
            }
            catch (Exception e)
            {
                // ReSharper disable once AsyncVoidMethod
                throw new Exception($"Ошибка при загрузке данных {e}");
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
                    GroupId = selectedGroup.Id
                };

                await _studentService.CreateStudent(student);
                MessageBox.Show("Студент успешно добавлен");

                LoadData();
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении студента");
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

                selectedStudent.FirstName = nameTxt.Text;
                selectedStudent.LastName = surnameTxt.Text;
                selectedStudent.MiddleName = middleName.Text;
                selectedStudent.GroupId = selectedGroup.Id;

                var user = await _userService.GetUserInfo(selectedStudent.UserId);
                user.UserName = loginTxt.Text;
                user.PasswordHash = passTxt.Text;
                await _userService.UpdateUser(user);

                await _studentService.UpdateStudent(selectedStudent);
                MessageBox.Show("Данные обновлены");

                LoadData();
            }
            catch
            {
                MessageBox.Show("Ошибка при редактировании студента");
            }
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is not Student selectedStudent)
                    return;

                nameTxt.Text = selectedStudent.FirstName;
                surnameTxt.Text = selectedStudent.LastName;
                middleName.Text = selectedStudent.MiddleName!;

                GroupComboBox.SelectedValue = selectedStudent.GroupId;

                var user = _userService.GetUserInfo(selectedStudent.UserId).Result;
                loginTxt.Text = user.UserName;
                passTxt.Text = user.PasswordHash;
            }
            catch
            {
                MessageBox.Show("Ошибка при выборе строки");
            }
        }
    }
}