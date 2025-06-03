using System.Windows;
using ElectronicJournal.WPF;
using ElectronicJournal.Domain;
using ElectronicJournal.WPF.Views;

namespace ElectronicJournal.Views
{
	/// <summary>
	/// Логика взаимодействия для AdminWin.xaml
	/// </summary>
	public partial class AdminWin : Window
	{
        ApplicationDbContext journalEntities = new ApplicationDbContext();
		public AdminWin()
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Schedule.ToList();
			
		}
		private void AddTeacherBt_Click(object sender, RoutedEventArgs e)
		{
			AddTeacherWin addTeacherWin = new AddTeacherWin();
			addTeacherWin.ShowDialog();
		}
		private void SheduleBt_Click(object sender, RoutedEventArgs e)
		{
			ScheduleWin scheduleWin = new ScheduleWin();
			scheduleWin.ShowDialog();
		}
		private void ListSheduleBt_Click(object sender, RoutedEventArgs e)
		{
			dataGrid.ItemsSource = journalEntities.Schedule.Where(m => m.Class.nameClass == klassTxt.Text).ToList();
		}
		private void AddStudentBt_Click(object sender, RoutedEventArgs e)
		{
			AddStudent addStudent = new AddStudent();
			addStudent.ShowDialog();
		}

		private void AddGroupBt_Click(object sender, RoutedEventArgs e)
		{
			AddGroup addGroup = new AddGroup();
			addGroup.ShowDialog();
		}

		private void AddDiscBt_Click(object sender, RoutedEventArgs e)
		{
			AddDiscipline addDiscipline = new AddDiscipline();
			addDiscipline.ShowDialog();
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			MainWindow mainWindow = new MainWindow();
			Close();
			mainWindow.Show();
		}
	}
}
