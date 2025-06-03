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
	/// Логика взаимодействия для ScheduleWin.xaml
	/// </summary>
	public partial class ScheduleWin : Window
	{
		ElectJournalEntities journalEntities =new ElectJournalEntities();
		public ScheduleWin()
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Schedule.ToList();
		}

		private void ListSheduleBt_Click(object sender, RoutedEventArgs e)
		{
			dataGrid.ItemsSource = journalEntities.Schedule.Where(m => m.Class.nameClass == klassTxt.Text).ToList();
		}

		private void AddStudentBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var id = journalEntities.Class.Where(m => m.nameClass == klTxt.Text).Single();
				var ids = journalEntities.Teacher.Where(k => k.Discipline.nameDiscipline == discTxt.Text).Single();
				Schedule schedule = new Schedule() 
				{
					idClass = id.idClass,
					day = dayOnWeek.Text,
					idTeacher = ids.idTeacher
				};
				journalEntities.Schedule.Add(schedule);
				journalEntities.SaveChanges();
				dataGrid.ItemsSource = journalEntities.Schedule.ToList();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
		}

		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var p = dataGrid.SelectedItem as Schedule;
				var id = journalEntities.Class.Where(m => m.nameClass == klTxt.Text).Single();
				var ids = journalEntities.Teacher.Where(k => k.Discipline.nameDiscipline == discTxt.Text).Single();
				p.idClass = id.idClass;
				p.day = dayOnWeek.Text;
				p.idTeacher = ids.idTeacher;
				journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = journalEntities.Schedule.ToList();
		}
		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Schedule;
				klTxt.Text = x.Class.nameClass;
				dayOnWeek.Text = x.day;
				discTxt.Text = x.Teacher.Discipline.nameDiscipline;
			}
			catch { }
		}
	}
}
