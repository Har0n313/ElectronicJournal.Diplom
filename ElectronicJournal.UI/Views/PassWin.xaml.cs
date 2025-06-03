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
	/// Логика взаимодействия для PassWin.xaml
	/// </summary>
	public partial class PassWin : Window
	{
		public int? IdDisc { get; set; }
		public string kls { get; set; }
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public PassWin(string klass, int? idDisc)
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Student.Where(m=>m.Class.nameClass == klass).ToList();
			IdDisc = idDisc;
			kls = klass;
		}
		private void saveBt_Click(object sender, RoutedEventArgs e)
		{
			var x = dataGrid.SelectedItem as Student;
			var id = journalEntities.Type.Where(m => m.nameType == passTxt.Text).Single();
			Pass pass = new Pass()
			{
				idStudent = x.idStudent,
				idType = id.idType,
				data = Convert.ToDateTime(dateTxt.Text),
				idDiscipline = IdDisc,
			};
			journalEntities.Pass.Add(pass);
			journalEntities.SaveChanges();
			dataGrid.ItemsSource = journalEntities.Student.Where(m => m.Class.nameClass == kls).ToList();
		}

		private void listBt_Click(object sender, RoutedEventArgs e)
		{
			ListPass listPass = new ListPass(IdDisc);
			listPass.ShowDialog();
		}
	}
}
