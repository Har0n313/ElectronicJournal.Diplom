using System.Windows;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Enums;
using ElectronicJournal.Views;

namespace ElectronicJournal.WPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ApplicationDbContext electronicJournal = new ApplicationDbContext();
		public MainWindow()
		{
			InitializeComponent();
		}
		private void EnterBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var id = electronicJournal.Users.Where(m=>m.Username == loginTxt.Text && m.PasswordHash == passwordTxt.Password).Single();
				if (id.Role == UserRole.Admin)
				{
					AdminWin adminWin = new AdminWin();
					Close();
					adminWin.Show();
				}
				else if(id.Role == UserRole.Teacher)
				{
					var idt = electronicJournal.Teachers.Where(m => m.Id == id.Id).Single();
					TeacherWin teacherWin = new TeacherWin(idt.Id, idt.Id);
					Close();
					teacherWin.Show();
				}
				else if (id.Role == UserRole.Student)
				{
					var ids = electronicJournal.Students.Where(m => m.Id == id.Id).Single();
					StudentWin studentWin = new StudentWin(ids.Id);
					Close();
					studentWin.Show();
				}
			}
			catch
			{
				MessageBox.Show("Не правильный логин или пароль");
			}
		}
	}
}
