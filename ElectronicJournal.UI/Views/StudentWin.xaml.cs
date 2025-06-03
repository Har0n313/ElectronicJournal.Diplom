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
	/// Логика взаимодействия для StudentWin.xaml
	/// </summary>
	public partial class StudentWin : Window
	{
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		string name, surname, cls;

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			MainWindow mainWindow = new MainWindow();
			Close();
			mainWindow.Show();
        }

        public StudentWin(int idStudent)
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Assessment.Where(m => m.idStudent == idStudent).ToList();
			dataGrid.IsReadOnly = true;
			var x = journalEntities.Student.Where(m=>m.idStudent == idStudent).Single();
			name = x.nameStudent;
			surname = x.surnameStudent;
			cls = x.Class.nameClass;
			namelb.Content = $"{surname} {name} {cls}";
		}
	}
}
