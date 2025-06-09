using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;
using System.Windows;
using System.Windows.Controls;

namespace ElectronicJournal.WPF.Views
{
    public partial class ScheduleWin
    {
        private readonly IScheduleService _scheduleService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly ISubjectService _subjectService;
        private int _currentGroupId;

        public ScheduleWin(IScheduleService scheduleService, IGroupService groupService,
            ITeacherService teacherService, ISubjectService subjectService)
        {
            InitializeComponent();
            _scheduleService = scheduleService;
            _groupService = groupService;
            _teacherService = teacherService;
            _subjectService = subjectService;
        }

        private async void ListSheduleBt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(klassTxt.Text))
            {
                MessageBox.Show("Введите группу");
                return;
            }

            var group = (await _groupService.GetAllGroups()).FirstOrDefault(g => g.Name == klassTxt.Text);
            if (group == null)
            {
                MessageBox.Show("Группа не найдена");
                return;
            }

            _currentGroupId = group.Id;

            await LoadScheduleForDay(DayOfWeek.Monday, mondayGrid);
            await LoadScheduleForDay(DayOfWeek.Tuesday, tuesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Wednesday, wednesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Thursday, thursdayGrid);
            await LoadScheduleForDay(DayOfWeek.Friday, fridayGrid);
            await LoadScheduleForDay(DayOfWeek.Saturday, saturdayGrid);
        }

        private async Task LoadScheduleForDay(DayOfWeek day, DataGrid grid)
        {
            var data = await _scheduleService.GetScheduleByDay(_currentGroupId, day);
            var formatted = data.OrderBy(s => s.PairNumber).Select(s => new
            {
                s.Id,
                s.PairNumber,
                Time = $"{s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm}",
                Subject = s.SubjectAssignment?.Subject?.Name ?? "",
                Teacher = FormatTeacherName(s.SubjectAssignment?.Teacher),
                s.Room
            });
            grid.ItemsSource = formatted.ToList();
        }

        private string FormatTeacherName(Teacher teacher)
        {
            if (teacher == null) return "";
            return $"{teacher.LastName} {teacher.FirstName[0]}. {teacher.MiddleName?[0]}.".TrimEnd('.');
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var schedule = await TryParseScheduleInputAsync();
            if (schedule == null)
                return;

            await _scheduleService.AddSchedule(schedule);
            MessageBox.Show("Запись добавлена.");
            await ListSheduleBt_ClickReload();
        }


        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var schedule = await TryParseScheduleInputAsync();
            if (schedule == null)
                return;

            var all = await _scheduleService.GetScheduleByDay(_currentGroupId, schedule.Day);
            var existing = all.FirstOrDefault(s => s.PairNumber == schedule.PairNumber);

            if (existing == null)
            {
                MessageBox.Show("Запись не найдена для редактирования.");
                return;
            }

            schedule.Id = existing.Id;
            await _scheduleService.UpdateSchedule(schedule);
            await ListSheduleBt_ClickReload();
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(pairNumberTxt.Text, out int pairNum) || !Enum.TryParse(dayTxt.Text, out DayOfWeek day))
            {
                MessageBox.Show("Введите корректные данные дня и номера пары");
                return;
            }

            var entries = await _scheduleService.GetScheduleByDay(_currentGroupId, day);
            var entry = entries.FirstOrDefault(s => s.PairNumber == pairNum);
            if (entry == null)
            {
                MessageBox.Show("Запись не найдена");
                return;
            }

            await _scheduleService.DeleteSchedule(entry.Id);
            await ListSheduleBt_ClickReload();
        }

        private async Task ListSheduleBt_ClickReload()
        {
            await LoadScheduleForDay(DayOfWeek.Monday, mondayGrid);
            await LoadScheduleForDay(DayOfWeek.Tuesday, tuesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Wednesday, wednesdayGrid);
            await LoadScheduleForDay(DayOfWeek.Thursday, thursdayGrid);
            await LoadScheduleForDay(DayOfWeek.Friday, fridayGrid);
            await LoadScheduleForDay(DayOfWeek.Saturday, saturdayGrid);
        }

        private async Task<Schedule?> TryParseScheduleInputAsync()
        {
            if (!Enum.TryParse(dayTxt.Text, out DayOfWeek day) ||
                !int.TryParse(pairNumberTxt.Text, out int pairNumber) ||
                string.IsNullOrWhiteSpace(timeTxt.Text) ||
                string.IsNullOrWhiteSpace(discTxt.Text) ||
                string.IsNullOrWhiteSpace(roomTxt.Text))
            {
                MessageBox.Show("Заполните все поля корректно.");
                return null;
            }

            var subjects = await _subjectService.GetAllSubjects();
            var subject = subjects.FirstOrDefault(s => s.Name == discTxt.Text);

            var teacherId = (int?)teacherComboBox.SelectedValue;
            if (teacherId == null)
            {
                MessageBox.Show("Выберите преподавателя из списка.");
                return null;
            }

            var teacher = await _teacherService.GetTeacherById(teacherId.Value);

            if (subject == null || teacher == null)
            {
                MessageBox.Show("Предмет или преподаватель не найден.");
                return null;
            }

            var assignment = teacher.SubjectAssignments.FirstOrDefault(sa =>
                sa.SubjectId == subject.Id && sa.GroupId == _currentGroupId);

            if (assignment == null)
            {
                MessageBox.Show("Назначение преподавателя на предмет в этой группе не найдено.");
                return null;
            }

            var timeParts = timeTxt.Text.Split('-');
            if (timeParts.Length != 2 || 
                !TimeSpan.TryParse(timeParts[0], out var start) || 
                !TimeSpan.TryParse(timeParts[1], out var end))
            {
                MessageBox.Show("Неверный формат времени. Пример: 08:30-10:00");
                return null;
            }

            return new Schedule
            {
                Day = day,
                PairNumber = pairNumber,
                StartTime = start,
                EndTime = end,
                Room = roomTxt.Text,
                SubjectAssignmentId = assignment.Id
            };
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTeachers();
        }

        private async Task LoadTeachers()
        {
            var teachers = await _teacherService.GetAllTeachers();
            teacherComboBox.ItemsSource = teachers.Select(t => new
            {
                t.Id,
                FullName = $"{t.LastName} {t.FirstName[0]}. {t.MiddleName?[0]}.".TrimEnd('.')
            }).ToList();
        }
    }
}
