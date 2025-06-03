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
	/// Логика взаимодействия для ListPass.xaml
	/// </summary>
	public partial class ListPass : Window
	{
		public int? IdDisc { get; set; }
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public ListPass(int? idDisc)
		{
			InitializeComponent();
			dataGrid.ItemsSource = journalEntities.Pass.Where(m=>m.idDiscipline == idDisc).ToList();
			dataGrid.IsReadOnly = true;
			IdDisc = idDisc;
		}

		private void searchBt_Click(object sender, RoutedEventArgs e)
		{
			var date = Convert.ToDateTime(dateTxt.Text);
			dataGrid.ItemsSource = journalEntities.Pass.Where(m=>m.data == date && m.idDiscipline == IdDisc).ToList();
		}
	}
}
