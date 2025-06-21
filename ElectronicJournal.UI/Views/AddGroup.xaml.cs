using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain.Entites;

namespace ElectronicJournal.WPF.Views;

/// <summary>
/// Логика взаимодействия для AddGroup.xaml
/// </summary>
public partial class AddGroup
{
    private readonly IGroupService _groupService;
    private Group? _selectedGroup;

    public AddGroup(IGroupService service)
    {
        _groupService = service;
        InitializeComponent();
        LoadData();
        ClearFields();
    }

    private async void LoadData()
    {
        try
        {
            var groupps = await _groupService.GetAllAsync();
            dataGrid.ItemsSource = groupps.OrderBy(g => g.Name);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private async void AddBt_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Group group = new()
            {
                Name = dicNameTxt.Text,
                NameSpecialty = discTxt.Text,
                AdmissionYear = int.Parse(yearsTxt.Text),
                SpecialtyCode = codedisTxt.Text,
            };
            await _groupService.CreateGroup(group);
        }
        catch
        {
            MessageBox.Show("Не все данные были введены корректно");
        }

        LoadData();
        ClearFields();
    }

    private async void EditBt_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _selectedGroup = dataGrid.SelectedItem as Group;
            if (_selectedGroup == null)
            {
                MessageBox.Show("Пожалуйста, выберите группу для редактирования.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _selectedGroup.Name = dicNameTxt.Text;
            _selectedGroup.NameSpecialty = discTxt.Text;
            _selectedGroup.AdmissionYear = int.Parse(yearsTxt.Text);
            _selectedGroup.SpecialtyCode = codedisTxt.Text;
            await _groupService.UpdateGroup(_selectedGroup);
            LoadData();
            ClearFields();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (KeyNotFoundException ex)
        {
            MessageBox.Show(ex.Message, "Не найдено", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка при обновлении группы: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        ClearFields();
    }

    private async void DeletBt_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedGroup == null)
        {
            MessageBox.Show("Выберите группу для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show("Вы уверены, что хотите удалить группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _groupService.DeleteGroup(_selectedGroup.Id);
            LoadData();
            ClearFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        try
        {
            _selectedGroup = dataGrid.SelectedItem as Group;
            dicNameTxt.Text = _selectedGroup!.Name;
            codedisTxt.Text = _selectedGroup!.SpecialtyCode;
            discTxt.Text = _selectedGroup.NameSpecialty;
            yearsTxt.Text = _selectedGroup.AdmissionYear.ToString();
        }
        catch
        {
            // ignored
        }
    }

    private void ClearFields()
    {
        discTxt.Text = string.Empty;
        codedisTxt.Text = string.Empty;
        dicNameTxt.Text = string.Empty;
        yearsTxt.Text = string.Empty;
        _selectedGroup = null;
    }
}