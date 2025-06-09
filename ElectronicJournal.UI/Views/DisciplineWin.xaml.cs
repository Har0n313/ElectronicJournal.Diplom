using System.Collections;
using ElectronicJournal.Application.Interfaces;

namespace ElectronicJournal.WPF.Views
{
	/// <summary>
	/// Логика взаимодействия для DisciplineWin.xaml
	/// </summary>
	public partial class DisciplineWin
	{
		private readonly ITeacherService _teacherService;
		public DisciplineWin(int idTeacher, ITeacherService teacherService)
		{
			_teacherService = teacherService;
			InitializeComponent();
			DataGridLoad(idTeacher);
		}

		private async void DataGridLoad(int idTeacher)
		{
			try
			{
				var x = await _teacherService.GetTeacherById(idTeacher);
				dataGrid.ItemsSource = (IEnumerable)x;
				dataGrid.IsReadOnly = true;
			}
			catch (Exception e)
			{
				throw; // TODO handle exception
			}
		}
	}
}
