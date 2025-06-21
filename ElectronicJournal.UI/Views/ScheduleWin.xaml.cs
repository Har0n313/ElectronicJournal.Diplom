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
            if (GroupComboBox.SelectedItem is not Group group)
            {
                MessageBox.Show("Выберите группу");
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
                Time = $"{s.StartTime} - {s.EndTime}",
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
            if (GroupComboBox.SelectedItem is not Group selectedGroup ||
                dayComboBox.SelectedItem is not DayOfWeek day ||
                PairNumberComboBox.SelectedItem is not int pairNumber ||
                AssignmentComboBox.SelectedValue is not int assignmentId ||
                string.IsNullOrWhiteSpace(roomTxt.Text))
            {
                MessageBox.Show("Заполните все поля корректно.");
                return null;
            }

            var times = (await _scheduleService.GetPairTimes(pairNumber)).ToList();
            if (times.Count != 2)
            {
                MessageBox.Show("Не удалось получить время для выбранного номера пары.");
                return null;
            }

            return new Schedule
            {
                Day = day,
                PairNumber = pairNumber,
                StartTime = times[0],
                EndTime = times[1],
                Room = roomTxt.Text.Trim(),
                SubjectAssignmentId = assignmentId
            };
        }


        private void PairNumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PairNumberComboBox.SelectedItem is int selectedPair &&
                _pairTimes.TryGetValue(selectedPair, out var time))
            {
                TimeDisplayTextBlock.Text = $"{time.Start}-{time.End}";
            }
            else
            {
                TimeDisplayTextBlock.Text = string.Empty;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGroups();
            LoadDaysOfWeek();
            LoadPairNumbers();
        }

        private void LoadPairNumbers()
        {
            PairNumberComboBox.ItemsSource = _pairTimes.Keys.ToList();
        }


        private async void LoadGroups()
        {
            try
            {
                var groups = await _groupService.GetAllAsync();

                if (GroupComboBox != null)
                {
                    GroupComboBox.ItemsSource = groups.OrderBy(s => s.Name);
                    GroupComboBox.DisplayMemberPath = "Name";
                    GroupComboBox.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке групп: {ex.Message}");
            }
        }

        private void LoadDaysOfWeek()
        {
            dayComboBox.ItemsSource = Enum.GetValues(typeof(DayOfWeek));
        }


        private async void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupComboBox.SelectedItem is Group group)
            {
                _currentGroupId = group.Id;
                await LoadAssignmentsForGroup(group.Id);
            }
        }

        private async Task LoadAssignmentsForGroup(int groupId)
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachers(); // Важно: вместе с SubjectAssignments и Subject

                var assignments = teachers
                    .SelectMany(t => t.SubjectAssignments
                        .Where(sa => sa.GroupId == groupId)
                        .Select(sa => new
                        {
                            AssignmentId = sa.Id,
                            TeacherLastName = t.LastName,
                            Display = $"{t.LastName} — {sa.Subject?.Name ?? "Без предмета"}"
                        }))
                    .OrderBy(a => a.TeacherLastName) // Сортировка по фамилии
                    .Select(a => new
                    {
                        a.AssignmentId,
                        a.Display
                    })
                    .ToList();

                AssignmentComboBox.ItemsSource = assignments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке назначений: {ex.Message}");
            }
        }


        private readonly Dictionary<int, (string Start, string End)> _pairTimes = new()
        {
            { 1, ("8:30", "9:50") },
            { 2, ("10:00", "11:20") },
            { 3, ("11:30", "12:50") },
            { 4, ("13:20", "14:40") },
            { 5, ("14:50", "16:10") },
            { 6, ("16:20", "17:40") },
            { 7, ("17:50", "18:10") },
            { 8, ("18:20", "19:40") },
        };
    }
}