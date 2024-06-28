using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using InoAgenti.Models;
using InoAgenti.ViewModels;

namespace InoAgenti.Views.Add
{
    public partial class AddEditWindow : Window
    {
        private MainWindow _mainWindow;
        private MainWindowViewModel viewModel;
        private Bitmap _imagePath;
        private Agent _editingAgent;

        public AddEditWindow(MainWindow mainWindow, MainWindowViewModel viewModel, Agent editingAgent = null)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.viewModel = viewModel;
            _editingAgent = editingAgent;

            // Инициализация полей формы на основе агента для редактирования
            if (_editingAgent != null)
            {
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _editingAgent.Type);
                Name.Text = _editingAgent.Name;
                SalesCount.Text = _editingAgent.SalesCount.ToString();
                PhoneNumber.Text = _editingAgent.PhoneNumber;
                Email.Text = _editingAgent.Email;
                Priority.Text = _editingAgent.Priority.ToString();
                Address.Text = _editingAgent.Address;
                INN.Text = _editingAgent.INN;
                KPP.Text = _editingAgent.KPP;
                DirectorName.Text = _editingAgent.DirectorName;
                _imagePath = _editingAgent.ImagePath;

                CreateAgentButton.Content = "Сохранить изменения";
                AddPhotoButton.Content = "Изменить фото";
                DeleteAgentButton.IsVisible = true;
            }
            else
            {
                CreateAgentButton.Content = "Создать агента";
                AddPhotoButton.Content = "Добавить фото";
                DeleteAgentButton.IsVisible = false;
            }

            Closed += OnClosed; // Добавляем обработчик события закрытия окна
        }

        private async void AddPhoto(object? sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "Выберите изображение",
                Directory = "C:\\Users\\glkho\\RiderProjects\\Bindings\\Bindings\\Assets",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "jpeg", "ico" } }
                }
            };

            var result = await fileDialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                var imagePath = result[0];
                var bitmap = new Bitmap(imagePath);
                _imagePath = bitmap;
            }
        }

        private async void CreateAgent(object? sender, RoutedEventArgs e)
{
    if (_imagePath == null)
    {
        _imagePath = new Bitmap("C:\\Users\\glkho\\RiderProjects\\Agents\\InoAgenti\\Assets\\picture.png");
    }

    if (_editingAgent != null)
    {
        _editingAgent.Type = (string)((ComboBoxItem)TypeComboBox.SelectedItem).Content;
        _editingAgent.Name = Name.Text;
        _editingAgent.SalesCount = int.Parse(SalesCount.Text);
        _editingAgent.PhoneNumber = PhoneNumber.Text;
        _editingAgent.Email = Email.Text;
        _editingAgent.Priority = int.Parse(Priority.Text);
        _editingAgent.Address = Address.Text;
        _editingAgent.INN = INN.Text;
        _editingAgent.KPP = KPP.Text;
        _editingAgent.DirectorName = DirectorName.Text;
        _editingAgent.ImagePath = _imagePath;

        // Вычисляем размер скидки
        _editingAgent.CalculateDiscount();

        Close();
    }
    else
    {
        var newAgent = new Agent()
        {
            Type = (string)((ComboBoxItem)TypeComboBox.SelectedItem).Content,
            Name = Name.Text,
            SalesCount = int.Parse(SalesCount.Text),
            PhoneNumber = PhoneNumber.Text,
            Email = Email.Text,
            Priority = int.Parse(Priority.Text),
            Address = Address.Text,
            INN = INN.Text,
            KPP = KPP.Text,
            DirectorName = DirectorName.Text,
            ImagePath = _imagePath
        };

        // Вычисляем размер скидки
        newAgent.CalculateDiscount();
        
        if (viewModel._agents.Any(p => p.Name == newAgent.Name))
        {
            Error.Text = $"Иноагент с именем '{newAgent.Name}' уже существует.";
            await Task.Delay(3000);
            Error.Text = string.Empty;
            return;
        }

        viewModel.AddAgent(newAgent);
    }

    // Очищаем поля формы после сохранения агента
    Name.Text = string.Empty;
    SalesCount.Text = string.Empty;
    PhoneNumber.Text = string.Empty;
    Email.Text = string.Empty;
    Priority.Text = string.Empty;
    Address.Text = string.Empty;
    INN.Text = string.Empty;
    KPP.Text = string.Empty;
    DirectorName.Text = string.Empty;
    Close();
}

        private void DeleteAgent(object? sender, RoutedEventArgs e)
        {
            if (_editingAgent != null)
            {
                viewModel._agents.Remove(_editingAgent);
                viewModel._allAgents.Remove(_editingAgent);
                _mainWindow.Agent.ItemsSource = null;
                _mainWindow.Agent.ItemsSource = viewModel._agents;
                Close();
            }
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            _mainWindow.DataContext = null;
            _mainWindow.DataContext = viewModel;
            _mainWindow.Agent.ItemsSource = viewModel._agents;
        }
    }
}