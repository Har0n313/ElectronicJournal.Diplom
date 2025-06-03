using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views
{
	/// <summary>
	/// Логика взаимодействия для AddDiscipline.xaml
	/// </summary>
	public partial class AddDiscipline
	{
		private readonly ApplicationDbContext _journalEntities = new();
		public AddDiscipline()
		{
			InitializeComponent();
			dataGrid.ItemsSource = _journalEntities.Subjects.ToList();
		}

		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			var x = dataGrid.SelectedItem as Subject;
			x!.Name = discTxt.Text;
			_journalEntities.SaveChanges();
			dataGrid.ItemsSource = _journalEntities.Subjects.ToList();

		}

		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var discipline = new Subject()
				{
					Name = discTxt.Text
				};
				_journalEntities.Subjects.Add(discipline);
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Subjects.ToList();

		}

		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Subject;
				discTxt.Text = x.Name;
			}
			catch
			{
				// ignored
			}
		}
	}
}
