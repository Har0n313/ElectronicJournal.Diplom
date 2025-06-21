using System.Windows;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Enums;
using ElectronicJournal.WPF.Views;

namespace ElectronicJournal.WPF;

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly IAssessmentService _assessmentService;
    private readonly IAttendanceService _attendanceService;
    private readonly IScheduleService _scheduleService;
    private readonly IHomeworkService _homeworkService;
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IUserService _userService;
    private readonly IStudentService _studentService;
    private readonly ISubjectAssignmentService _subjectAssignmentService;
    private readonly ILessonService _lessonService;

    public MainWindow() : this(null!, null!, null!, null!, null!, null!, null!,
        null!, null!, null!, null!)
    {
        InitializeComponent();
    }

    public MainWindow(ITeacherService teacherService, IUserService userService, IStudentService studentService,
        IAssessmentService assessmentService, IScheduleService scheduleService, IGroupService groupService,
        ISubjectService subjectService, IAttendanceService attendanceService, IHomeworkService homeworkService,
        ISubjectAssignmentService subjectAssignmentService, ILessonService lessonService)
    {
        _teacherService = teacherService;
        _userService = userService;
        _studentService = studentService;
        _assessmentService = assessmentService;
        _scheduleService = scheduleService;
        _groupService = groupService;
        _subjectService = subjectService;
        _attendanceService = attendanceService;
        _homeworkService = homeworkService;
        _subjectAssignmentService = subjectAssignmentService;
        _lessonService = lessonService;
        InitializeComponent();
    }

    private async void EnterBt_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var user = await _userService.Authenticate(loginTxt.Text, passwordTxt.Password);
            if (user.Role == UserRole.Admin)
            {
                AdminWin adminWin = new AdminWin(_scheduleService, _groupService, _teacherService, _subjectService,
                    _userService, _studentService, _subjectAssignmentService);

                adminWin.Show();
                adminWin.Owner = this;
                ClearTextBox();
                Hide();
            }
            else if (user.Role == UserRole.Teacher)
            {
                TeacherWin teacherWin = new TeacherWin(user.Teacher.Id, _teacherService, _assessmentService,
                    _subjectService, _groupService, _studentService, _attendanceService, _homeworkService,
                    _subjectAssignmentService, _lessonService);
                
                teacherWin.Show();
                teacherWin.Owner = this;
                ClearTextBox();
                Hide();
            }
            else if (user.Role == UserRole.Student)
            {
                StudentWin studentWin = new StudentWin(user.Student.Id, _assessmentService, _studentService,
                    _scheduleService, _attendanceService);

                studentWin.Show();
                studentWin.Owner = this;
                ClearTextBox();
                Hide();
            }
        }
        catch
        {
            MessageBox.Show("Не правильный логин или пароль");
        }
    }

    private void ClearTextBox()
    {
        loginTxt.Clear();
        passwordTxt.Clear();
    }
}