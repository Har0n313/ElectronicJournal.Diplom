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

namespace ElectronicJournal.Views
{
	/// <summary>
	/// Логика взаимодействия для HomeworkWin.xaml
	/// </summary>
	public partial class HomeworkWin : Window
	{
		public int idT { get; set; }
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public HomeworkWin(int idTeach)
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Homework.Where(m=>m.idTeacher == idTeach).ToList();
			idT = idTeach;
		}
		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			var k = journalEntities.Class.Where(m => m.nameClass == klassTxt.Text).Single();
			Homework homework = new Homework()
			{
				idClass = k.idClass,
				idTeacher = idT,
				task = taskTXT.Text,
				deadline = Convert.ToDateTime(deadlinetxt.Text)
			};
			journalEntities.Homework.Add(homework);
			journalEntities.SaveChanges();
			dataGrid.ItemsSource = journalEntities.Homework.Where(m => m.idTeacher == idT).ToList();
		}
	}
}
