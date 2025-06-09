using System.Windows;
using ElectronicJournal.Application.Interfaces;

namespace ElectronicJournal.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ListPass.xaml
    /// </summary>
    public partial class ListPass
    {
        private int IdDisc { get; set; }
        private readonly IAttendanceService _attendanceService;

        public ListPass(int idDisc, IAttendanceService attendanceService)
        {
            InitializeComponent();
            IdDisc = idDisc;
            _attendanceService = attendanceService;
            LoadData(idDisc);
        }

        private async void LoadData(int idDisc)
        {
            dataGrid.ItemsSource = await _attendanceService.GetAttendanceByLesson(idDisc);
            dataGrid.IsReadOnly = true;
        }

        private void searchBt_Click(object sender, RoutedEventArgs e)
        {
            var date = Convert.ToDateTime(dateTxt.Text);
            dataGrid.ItemsSource = journalEntities.Pass.Where(m => m.data == date && m.idDiscipline == IdDisc).ToList();
        }
    }
}