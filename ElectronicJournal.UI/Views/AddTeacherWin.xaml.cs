using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views
{
    public partial class AddTeacherWin
    {
        private readonly ITeacherService _teacherService;
        private readonly IUserService _userService;

        public AddTeacherWin(ITeacherService teacherService, IUserService userService)
        {
            _teacherService = teacherService;
            _userService = userService;
            InitializeComponent();
            LoadTeachers();
            dataGrid.IsReadOnly = true;
        }

        private async void LoadTeachers()
        {
            var teachers = await _teacherService.GetAllTeachers();
            dataGrid.ItemsSource = teachers;
        }

        private async void AddBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userDto = new User
                {
                    UserName = loginTxt.Text,
                    PasswordHash = passTxt.Text,
                    Role = UserRole.Teacher
                };

                var user = await _userService.RegisterUser(userDto);

                var teacherDto = new Teacher
                {
                    FirstName = nameTxt.Text,
                    LastName = surnameTxt.Text,
                    MiddleName = middleName.Text,
                    UserId = user.Id
                };

                await _teacherService.CreateTeacher(teacherDto);
                LoadTeachers();
            }
            catch
            {
                MessageBox.Show("Не все данные были введены корректно");
            }
        }

        private async void EditBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is Teacher selectedTeacher)
                {
                    var updatedTeacher = new Teacher
                    {
                        Id = selectedTeacher.Id,
                        FirstName = nameTxt.Text,
                        LastName = surnameTxt.Text,
                        MiddleName = middleName.Text
                    };

                    await _teacherService.UpdateTeacher(updatedTeacher);
                    LoadTeachers();
                }
            }
            catch
            {
                MessageBox.Show("Не все данные были введены корректно");
            }
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem is Teacher teacher)
                {
                    nameTxt.Text = teacher.FirstName;
                    surnameTxt.Text = teacher.LastName;
                    middleName.Text = teacher.MiddleName;
                    loginTxt.Text = teacher.User.UserName;
                    passTxt.Text = ""; // безопаснее не показывать пароль
                }
            }
            catch
            {
                // Игнорируем исключение при неудачном выборе
            }
        }
    }
}