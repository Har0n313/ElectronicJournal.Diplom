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
	/// Логика взаимодействия для DisciplineWin.xaml
	/// </summary>
	public partial class DisciplineWin : Window
	{
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public DisciplineWin(int idTeacher)
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Teacher.Where(m=>m.idTeacher == idTeacher).ToList();
			dataGrid.IsReadOnly = true;
		}
	}
}
