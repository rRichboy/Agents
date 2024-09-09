using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

                CreateAgentButton.Content = "Сохранить";
                AddPhotoButton.Content = "Изменить фотокарточку";
                DeleteAgentButton.IsVisible = true;
            }
            else
            {
                CreateAgentButton.Content = "Добавить агента";
                AddPhotoButton.Content = "Добавить фотокарточку";
                DeleteAgentButton.IsVisible = false;
            }

            Closed += OnClosed;
        }
        
        private async void CreateAgent(object? sender, RoutedEventArgs e)
        {
            if (_imagePath == null)
            {
                var defaultImageUri = new Uri("avares://InoAgenti/Assets/picture.png");
                _imagePath = new Bitmap(AssetLoader.Open(defaultImageUri));
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
                
                newAgent.CalculateDiscount();

                if (viewModel._agents.Any(p => p.Name == newAgent.Name))
                {
                    Error.Text = $"Агент с именем '{newAgent.Name}' уже существует.";
                    await Task.Delay(3000);
                    Error.Text = string.Empty;
                    return;
                }

                viewModel.AddAgent(newAgent);
            }

            Close();
        }
        private async void AddPhoto(object? sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "Выберите изображение",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "jpeg", "ico" } }
                }
            };

            var result = await fileDialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                var imagePath = result[0];
                var uri = new Uri($"file://{imagePath}");
                var bitmap = new Bitmap(imagePath);
                _imagePath = bitmap;
            }
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