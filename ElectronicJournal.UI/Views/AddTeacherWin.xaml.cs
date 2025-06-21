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
            Loaded += AddTeacherWin_Loaded;
            DataGrid.IsReadOnly = true;
        }
        
        private async void AddTeacherWin_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTeachers();
        }

        private async Task LoadTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachers();
                DataGrid.ItemsSource = teachers.OrderBy(s=>s.LastName);
                Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void AddBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userDto = new User
                {
                    UserName = LoginTxt.Text,
                    PasswordHash = PassTxt.Text,
                    Role = UserRole.Teacher
                };

                var user = await _userService.RegisterUser(userDto);

                var teacherDto = new Teacher
                {
                    FirstName = NameTxt.Text,
                    LastName = SurnameTxt.Text,
                    MiddleName = MiddleName.Text,
                    UserId = user.Id,
                };
                await _teacherService.CreateTeacher(teacherDto);
                await LoadTeachers();
                
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
                if (DataGrid.SelectedItem is Teacher selectedTeacher && DataGrid.SelectedItem is User user)
                {
                    var updatedTeacher = new Teacher
                    {
                        Id = selectedTeacher.Id,
                        FirstName = NameTxt.Text,
                        LastName = SurnameTxt.Text,
                        MiddleName = MiddleName.Text
                    };

                    await _teacherService.UpdateTeacher(updatedTeacher);

                    var updateUser = new User()
                    {
                        Id = user.Id,
                        UserName = LoginTxt.Text,
                        PasswordHash = PassTxt.Text,
                    };
                    await _userService.UpdateUser(updateUser);
                    await LoadTeachers();
                }
            }
            catch
            {
                MessageBox.Show("Не все данные были введены корректно");
            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGrid.SelectedItem is Teacher selectedTeacher && DataGrid.SelectedItem is User user)
                {
                    await _teacherService.DeleteTeacher(selectedTeacher.Id);
                    await _userService.DeletUser(user.Id);
                    Clear();
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (DataGrid.SelectedItem is Teacher teacher)
                {
                    NameTxt.Text = teacher.FirstName;
                    SurnameTxt.Text = teacher.LastName;
                    MiddleName.Text = teacher.MiddleName ?? "";
                    LoginTxt.Text = teacher.User.UserName;
                    PassTxt.Text = teacher.User.PasswordHash; // безопаснее не показывать пароль
                }
            }
            catch
            {
                // Игнорируем исключение при неудачном выборе
            }
        }

        private void Clear()
        {
            NameTxt.Text = string.Empty;
            SurnameTxt.Text = string.Empty;
            MiddleName.Text = string.Empty;
            LoginTxt.Text = string.Empty;
            PassTxt.Text = string.Empty;
        }
    }
}