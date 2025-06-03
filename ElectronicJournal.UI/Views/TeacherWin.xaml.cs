using ElectronicJournal.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ElectronicJournal.WPF;

namespace ElectronicJournal.Views
{
	/// <summary>
	/// Логика взаимодействия для TeacherWin.xaml
	/// </summary>
	public partial class TeacherWin : Window
	{
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public int IdTeacher { get; set; }
		public int? IdDisc { get; set; }
		string surname, name, middleName;
		public TeacherWin(int idTeacher, int? idDisc)
		{
			InitializeComponent();
			IdTeacher = idTeacher;
			IdDisc = idDisc;
			var k = journalEntities.Teacher.Where(m => m.idTeacher == idTeacher).Single();
			surname = k.srnameTeacher;
			name = k.nameTeacher;
			middleName = k.middleNameTeacher;
			namelb.Content = $"Преподаватель: {surname} {name} {middleName}";
		}

		private void listBt_Click(object sender, RoutedEventArgs e)
		{
			dataGrid.ItemsSource = journalEntities.Assessment.Where(m => m.Student.Class.nameClass == classTxtBx.Text && m.idDiscipline == IdDisc).ToList();
		}

		private void passBt_Click(object sender, RoutedEventArgs e)
		{
			PassWin passWin = new PassWin(classTxtBx.Text, IdDisc);
			passWin.ShowDialog(); ;
		}

		private void HomeworkBt_Click(object sender, RoutedEventArgs e)
		{
			HomeworkWin homework = new HomeworkWin(IdTeacher);
			homework.ShowDialog();
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			MainWindow mainWindow = new MainWindow();
			Close();
			mainWindow.Show();
        }

        private void listDiscipline_Click(object sender, RoutedEventArgs e)
		{
			DisciplineWin disciplineWin = new DisciplineWin(IdTeacher);
			disciplineWin.ShowDialog();
		}

		private void editBt_Click(object sender, RoutedEventArgs e)
		{
			EditWin editWin = new EditWin(IdDisc,IdTeacher, classTxtBx.Text);
			Close();
			editWin.Show();
		}
	}
}
