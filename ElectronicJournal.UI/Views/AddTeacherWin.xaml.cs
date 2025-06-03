using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views
{
	/// <summary>
	/// Логика взаимодействия для AddTeacherWin.xaml
	/// </summary>
	public partial class AddTeacherWin
	{
		private readonly ApplicationDbContext _journalEntities = new();
		public AddTeacherWin()
		{
			InitializeComponent();
			dataGrid.ItemsSource = _journalEntities.Teachers.ToList();
			dataGrid.IsReadOnly = true;
		}

		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				User user = new User()
				{
					Username = loginTxt.Text,
					PasswordHash = passTxt.Text,
					Role = UserRole.Teacher
				};
				_journalEntities.Users.Add(user);
				_journalEntities.SaveChanges();
				var id = _journalEntities.Users.Where(m => m.Username == loginTxt.Text && m.PasswordHash == passTxt.Text).Single();
				var idc = _journalEntities.Subjects.Where(k => k.Name == DiscTxt.Text).Single();
				Teacher teacher = new Teacher()
				{
					FirstName = nameTxt.Text,
					LastName = surnameTxt.Text,
					MiddleName = middleName.Text,
					UserId = id.Id,
				};
				_journalEntities.Teachers.Add(teacher);
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Teachers.ToList();
		}

		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Teacher;
				var id = _journalEntities.Users.Where(m => m.Username == loginTxt.Text && m.PasswordHash == passTxt.Text).Single();
				var idc = _journalEntities.Discipline.Where(k => k.nameDiscipline == DiscTxt.Text).Single();
				x.nameTeacher = nameTxt.Text;
				x.srnameTeacher = surnameTxt.Text;
				x.middleNameTeacher = middleName.Text;
				x.idUser = id.idUser;
				x.idDiscipline = idc.idDiscipline;
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Teacher.ToList();
		}

		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Teacher;
				nameTxt.Text = x.nameTeacher;
				surnameTxt.Text = x.srnameTeacher;
				middleName.Text = x.middleNameTeacher;
				DiscTxt.Text = x.Discipline.nameDiscipline;
				loginTxt.Text = x.User.login;
				passTxt.Text = x.User.pass;
			}
			catch
			{ }
		}
	}
}
