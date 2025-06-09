using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Enums;
using ElectronicJournal.WPF.Views;

namespace ElectronicJournal.WPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly IAssessmentService _assessmentService;
        private readonly IScheduleService _scheduleService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        public MainWindow(ITeacherService teacherService, IUserService userService, IStudentService studentService, IAssessmentService assessmentService, IScheduleService scheduleService, IGroupService groupService, ISubjectService subjectService)
        {
	        _teacherService = teacherService;
	        _userService = userService;
	        _studentService = studentService;
	        _assessmentService = assessmentService;
	        _scheduleService = scheduleService;
	        _groupService = groupService;
	        _subjectService = subjectService;
	        InitializeComponent();
        }
		private async void EnterBt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var user = await _userService.Authenticate(loginTxt.Text, passwordTxt.Password);
				if (user.Role == UserRole.Admin)
				{
					AdminWin adminWin = new AdminWin(_scheduleService,_groupService, _teacherService, _subjectService, _userService);
					Close();
					adminWin.Show();
				}
				else if(user.Role == UserRole.Teacher)
				{
					var teacher = await _teacherService.GetTeacherById(user.Id);
					TeacherWin teacherWin = new TeacherWin(teacher.Id);
					Close();
					teacherWin.Show();
				}
				else if (user.Role == UserRole.Student)
				{
					var student = await _studentService.GetStudentById(user.Id);
					StudentWin studentWin = new StudentWin(student.Id, _assessmentService, _studentService);
					Close();
					studentWin.Show();
				}
			}
			catch
			{
				MessageBox.Show("Не правильный логин или пароль");
			}
		}
	}
}
