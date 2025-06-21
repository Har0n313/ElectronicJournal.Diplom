using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using ElectronicJournal.Domain;
using ElectronicJournal.WPF.Views;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.WPF
{
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>options.UseNpgsql("Host=localhost;Port=5432;Database=ElectronicJournalsV1;Username=postgres;Password=mysecretpassword"));
            
            services.AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    "Host=localhost;Port=5432;Database=ElectronicJournalsV1;Username=postgres;Password=mysecretpassword");
            });
            
            // Регистрация окон
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AdminWin>();
            services.AddSingleton<AddStudent>();
            services.AddSingleton<AddDiscipline>();
            services.AddSingleton<AddTeacherWin>();
            services.AddSingleton<AddGroup>();
            services.AddSingleton<DisciplineWin>();
            services.AddSingleton<EditWin>();
            services.AddSingleton<ListPass>();
            services.AddSingleton<PassWin>();
            services.AddSingleton<ScheduleWin>();
            services.AddSingleton<StudentWin>();
            services.AddSingleton<TeacherWin>();

            // Регистрация сервисов
            services.AddSingleton<IAssessmentService, AssessmentService>();
            services.AddSingleton<IAttendanceService, AttendanceService>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IHomeworkService, HomeworkService>();
            services.AddSingleton<ILessonService, LessonService>();
            services.AddSingleton<IScheduleService, ScheduleService>();
            services.AddSingleton<IStudentService, StudentService>();
            services.AddSingleton<ISubjectAssignmentService, SubjectAssignmentService>();
            services.AddSingleton<ISubjectService, SubjectService>();
            services.AddSingleton<ITeacherService, TeacherService>();
            services.AddSingleton<IUserService, UserService>();
        }
    }
}