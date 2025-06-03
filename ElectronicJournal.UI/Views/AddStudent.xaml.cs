using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.WPF.Views
{
	/// <summary>
	/// Логика взаимодействия для AddStudent.xaml
	/// </summary>
	public partial class AddStudent
	{
		private readonly ApplicationDbContext _journalEntities = new();
		public AddStudent()
		{
			InitializeComponent();
			dataGrid.ItemsSource = _journalEntities.Students.ToList();
			dataGrid.IsReadOnly = true;
		}

		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var user = new User()
				{
					Username = loginTxt.Text,
					PasswordHash = passTxt.Text,
					Role = UserRole.Student
				};
				_journalEntities.Users.Add(user);
				_journalEntities.SaveChanges();
				var id = _journalEntities.Users.Single(m => m.Username == loginTxt.Text && m.PasswordHash == passTxt.Text);
				var idc = _journalEntities.Groups.Single(k => k.Name == klsTxt.Text);
				Student student = new Student()
				{
					FirstName = nameTxt.Text,
					LastName = surnameTxt.Text,
					MiddleName = middleName.Text,
					UserId = id.Id,
					GroupId = idc.Id,
				};
				_journalEntities.Students.Add(student);
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Students.ToList();
		}

		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Student;
				var id = _journalEntities.Users.Single(m => m.Username == loginTxt.Text && m.PasswordHash == passTxt.Text);
				var idc = _journalEntities.Groups.Single(k => k.Name == klsTxt.Text);
				x.FirstName = nameTxt.Text;
				x.LastName = surnameTxt.Text;
				x.MiddleName = middleName.Text;
				x.UserId = id.Id;
				x.GroupId = idc.Id;
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Students.ToList();
		}

		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Student;
				nameTxt.Text = x.FirstName;
				surnameTxt.Text = x.LastName;
				middleName.Text = x.MiddleName;
				klsTxt.Text = x.Class.nameClass;
				loginTxt.Text = x.UserId.login;
				passTxt.Text = x.User.pass;
			}
			catch
			{
				// ignored
			}
		}
	}
}
