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
	/// Логика взаимодействия для EditWin.xaml
	/// </summary>
	public partial class EditWin : Window
	{
		ElectJournalEntities journalEntities = new ElectJournalEntities();
		public int? IdDisc {  get; set; }
		public int IdTeacher {  get; set; }
		public string klass { get; set; } 
		public EditWin(int? idDisc, int idTeach,string cl)
		{
			InitializeComponent();
			IdDisc = idDisc;
			IdTeacher = idTeach;
			klass = cl;
			Update();
		}
		public void Update()
		{
			dataGrid.ItemsSource = journalEntities.Assessment.Where(m => m.Student.Class.nameClass == klass && m.idDiscipline == IdDisc).ToList();
		}

		private void EditBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Assessment;
				var k = journalEntities.Type.Where(m => m.nameType == typeTxt.Text).Single();
				x.mark = int.Parse(markTxt.Text);
				x.idType = k.idType;
				x.date = Convert.ToDateTime(dataTxt.Text);
				journalEntities.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Не все данные были введены корректно");
			}
			Update();
		}

		private void BackBt_Click(object sender, RoutedEventArgs e)
		{
			TeacherWin teacherWin = new TeacherWin(IdTeacher,IdDisc);
			Close();
			teacherWin.Show();
		}

		private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Assessment;
				markTxt.Text = x.mark.ToString();
				dataTxt.Text = x.date.ToString();
				typeTxt.Text = x.Type.nameType.ToString();
			}
			catch
			{}
			Update();
		}

		private void AddBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var x = dataGrid.SelectedItem as Assessment;
				var k = journalEntities.Type.Where(m => m.nameType == typeTxt.Text).Single();
				Assessment assessment = new Assessment()
				{
					idStudent = x.idStudent,
					idDiscipline = IdDisc,
					mark = int.Parse(markTxt.Text),
					date = Convert.ToDateTime(dataTxt.Text),
					idType = k.idType,
				};
				journalEntities.Assessment.Add(assessment);
				journalEntities.SaveChanges();
				Update();
			}
			catch
			{

			}
        }
    }
}
