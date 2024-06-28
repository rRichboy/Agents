using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using InoAgenti.Models;
using InoAgenti.ViewModels;
using InoAgenti.Views.Add;

namespace InoAgenti.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Устанавливаем DataContext для привязки данных к ViewModel
        DataContext = new MainWindowViewModel();
        
        Agent.SelectionChanged += Agent_SelectionChanged;
    }

    // Метод для открытия окна добавления нового агента
    private void AddAgent(object? sender, RoutedEventArgs e)
    {
        // Создаем окно добавления/редактирования агента и передаем текущую ViewModel
        var dialog = new AddEditWindow(this, (MainWindowViewModel)DataContext);
        dialog.ShowDialog(this);
    }

    // Метод для открытия окна редактирования агента
    private void EditAgent(object? sender, TappedEventArgs e)
    {
        // Проверяем, что отправитель события - это ListBox, и выбранный элемент - это Agent
        if (sender is ListBox listBox && listBox.SelectedItem is Agent selectedAgent)
        {
            // Создаем окно добавления/редактирования агента и передаем текущую ViewModel и выбранного агента
            var dialog = new AddEditWindow(this, (MainWindowViewModel)DataContext, selectedAgent);
            dialog.ShowDialog(this);
        }
    }

    // Метод для фильтрации агентов
    private void filtration(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            // Получаем выбранный тип из ComboBox
            var selectedType = (string)((ComboBoxItem)TypeComboBox.SelectedItem)?.Content;

            // Если выбран "Все типы", сбрасываем фильтр
            if (selectedType == "Все типы")
            {
                selectedType = null;
            }

            // Применяем фильтрацию и обновляем источник данных для ListBox
            var filteredProducts = viewModel.filtration(selectedType);
            Agent.ItemsSource = filteredProducts;
        }
    }

    // Метод для сортировки агентов
    private void Sort(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            // Получаем выбранный вариант сортировки из ComboBox
            var sortOption = (string)((ComboBoxItem)SortComboBox.SelectedItem)?.Content;

            // Если выбран "Сортировка", сбрасываем сортировку
            if (sortOption == "Сортировка")
            {
                sortOption = null;
            }

            // Применяем сортировку и обновляем источник данных для ListBox
            var sortedProducts = viewModel.Sort(sortOption);
            Agent.ItemsSource = sortedProducts;
        }
    }

    // Метод для поиска агентов
    private void Search_KeyUp(object sender, KeyEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            // Получаем текст для поиска
            var searchText = Search.Text;
            // Применяем поиск и обновляем источник данных для ListBox
            var searchedProducts = viewModel.Search(searchText);
            Agent.ItemsSource = searchedProducts;
        }
    }
    
    // Обработчик изменения выбора агентов
    private void Agent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ChangePriorityButton.IsVisible = Agent.SelectedItems.Count > 2;
    }
    
    // Обработка нажатия кнопки "Изменить приоритет на ..."
    private async void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
    {
        if (Agent.SelectedItems.Count >= 2)
        {
            var selectedAgents = Agent.SelectedItems.Cast<Agent>();
            var maxPriority = selectedAgents.Max(agent => agent.Priority);

            var dialog = new ChangePriorityWindow(maxPriority);
            await dialog.ShowDialog(this);

            foreach (var agent in selectedAgents)
            {
                agent.Priority = dialog.NewPriority;
            }

            // Обновляем отображение
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.UpdateAgents(new ObservableCollection<Agent>(viewModel.Agents));
                Agent.ItemsSource = viewModel.Agents; // Обновляем источник данных ListBox

                ChangePriorityButton.IsVisible = false;
            }
        }
    }
}