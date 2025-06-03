using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views
{
	/// <summary>
	/// Логика взаимодействия для AddGroup.xaml
	/// </summary>
	public partial class AddGroup : Window
	{
		private readonly ApplicationDbContext _journalEntities = new();
		public AddGroup()
		{
			InitializeComponent();
			dataGrid.ItemsSource = _journalEntities.Groups.ToList();
		}

		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Group;
				discTxt.Text = x.Name;
			}
			catch
			{ }
		}

		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Group class1 = new()
				{
					Name = discTxt.Text,
				};
				_journalEntities.Groups.Add(class1);
				_journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			dataGrid.ItemsSource = _journalEntities.Groups.ToList();

		}
		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			var x = dataGrid.SelectedItem as Group;
			x.Name = discTxt.Text;
			_journalEntities.SaveChanges();
			dataGrid.ItemsSource = _journalEntities.Groups.ToList();

		}
	}
}
