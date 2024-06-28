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
        DataContext = new MainWindowViewModel();
        Agent.SelectionChanged += Agent_SelectionChanged;
    }
    private void AddAgent(object? sender, RoutedEventArgs e)
    {
        var dialog = new AddEditWindow(this, (MainWindowViewModel)DataContext);
        dialog.ShowDialog(this);
    }
    private void EditAgent(object? sender, TappedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is Agent selectedAgent)
        {
            var dialog = new AddEditWindow(this, (MainWindowViewModel)DataContext, selectedAgent);
            dialog.ShowDialog(this);
        }
    }
    private void filtration(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            var selectedType = (string)((ComboBoxItem)TypeComboBox.SelectedItem)?.Content;
            viewModel.filtration(selectedType);
        }
    }
    private void Sort(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            var sortOption = (string)((ComboBoxItem)SortComboBox.SelectedItem)?.Content;
            viewModel.Sort(sortOption);
        }
    }
    private void Search_KeyUp(object sender, KeyEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            var searchText = Search.Text;
            viewModel.Search(searchText);
        }
    }
    private void Agent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ChangePriorityButton.IsVisible = Agent.SelectedItems.Count > 2;
    }
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

            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.UpdateAgents(new ObservableCollection<Agent>(viewModel._agents));
                Agent.ItemsSource = viewModel._agents; 
                ChangePriorityButton.IsVisible = false;
            }
        }
    }
    private void PreviousPage_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            viewModel.PreviousPage_Click(sender, e);
        }
    }
    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            viewModel.NextPage_Click(sender, e);
        }
    } 
    private void PageNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel != null)
        {
            viewModel.PageNumber_SelectionChanged(sender, e);
        }
    }
}