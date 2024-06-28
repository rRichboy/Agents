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
        private MainWindow _mainWindow; // Главное окно
        private MainWindowViewModel viewModel; // ViewModel главного окна
        private Bitmap _imagePath; // Путь к изображению
        private Agent _editingAgent; // Агент, который редактируется

        // Конструктор окна добавления/редактирования агента
        public AddEditWindow(MainWindow mainWindow, MainWindowViewModel viewModel, Agent editingAgent = null)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            this.viewModel = viewModel;
            _editingAgent = editingAgent;

            // Если редактируется существующий агент, заполняем поля его данными
            if (_editingAgent != null)
            {
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _editingAgent.Type);
                Name.Text = _editingAgent.Name;
                SalesCount.Text = _editingAgent.SalesCount.ToString();
                PhoneNumber.Text = _editingAgent.PhoneNumber;
                Email.Text = _editingAgent.Email;
                Priority.Text = _editingAgent.Priority.ToString();
                SuccessRate.Text = _editingAgent.SuccessRate.ToString();
                _imagePath = _editingAgent.ImagePath;

                CreateAgentButton.Content = "Сохранить изменения"; // Изменяем текст кнопки
                AddPhotoButton.Content = "Изменить фото"; // Изменяем текст кнопки
                DeleteAgentButton.IsVisible = true; // Делаем кнопку удаления видимой
            }
            else
            {
                CreateAgentButton.Content = "Создать агента"; // Устанавливаем текст кнопки
                AddPhotoButton.Content = "Добавить фото"; // Устанавливаем текст кнопки
                DeleteAgentButton.IsVisible = false; // Скрываем кнопку удаления
            }

            this.Closed += OnClosed; // Добавляем обработчик события закрытия окна
        }
        
        // Метод для добавления фото
        private async void AddPhoto(object? sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                AllowMultiple = false, // Разрешаем выбрать только один файл
                Title = "Выберите изображение", // Заголовок окна выбора файла
                Directory = "C:\\Users\\glkho\\RiderProjects\\Bindings\\Bindings\\Assets", // Директория по умолчанию
                Filters = new List<FileDialogFilter> // Фильтры для типов файлов
                {
                    new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "jpeg", "ico" } }
                }
            };

            var result = await fileDialog.ShowAsync(this); // Показываем диалоговое окно

            if (result != null && result.Length > 0)
            {
                var imagePath = result[0];
                var bitmap = new Bitmap(imagePath);
                _imagePath = bitmap; // Сохраняем путь к выбранному изображению
            }
        }

        // Метод для создания или редактирования агента
        private async void CreateAgent(object? sender, RoutedEventArgs e)
        {
            // Если изображение не выбрано, устанавливаем изображение по умолчанию
            if (_imagePath == null)
            {
                _imagePath = new Bitmap("C:\\Users\\glkho\\OneDrive\\Рабочий стол\\Сессия\\InoAgenti\\InoAgenti\\Assets\\picture.png");
            }

            // Если редактируется существующий агент
            if (_editingAgent != null)
            {
                _editingAgent.Type = (string)((ComboBoxItem)TypeComboBox.SelectedItem).Content;
                _editingAgent.Name = Name.Text;
                _editingAgent.SalesCount = int.Parse(SalesCount.Text);
                _editingAgent.PhoneNumber = PhoneNumber.Text;
                _editingAgent.Email = Email.Text;
                _editingAgent.Priority = int.Parse(Priority.Text);
                _editingAgent.SuccessRate = int.Parse(SuccessRate.Text);
                _editingAgent.ImagePath = _imagePath;

                Close(); // Закрываем окно после сохранения изменений
            }
            else
            {
                // Создаем нового агента
                var newAgent = new Agent()
                {
                    Type = (string)((ComboBoxItem)TypeComboBox.SelectedItem).Content,
                    Name = Name.Text,
                    SalesCount = int.Parse(SalesCount.Text),
                    PhoneNumber = PhoneNumber.Text,
                    Email = Email.Text,
                    Priority = int.Parse(Priority.Text),
                    SuccessRate = int.Parse(SuccessRate.Text),
                    ImagePath = _imagePath
                };

                // Проверяем, существует ли агент с таким же именем
                if (viewModel.Agents.Any(p => p.Name == newAgent.Name))
                {
                    Error.Text = $"Иноагент с именем '{newAgent.Name}' уже существует.";
                    await Task.Delay(3000);
                    Error.Text = string.Empty; // Очищаем сообщение об ошибке
                    return;
                }

                viewModel.AddAgent(newAgent); // Добавляем нового агента в коллекцию
            }

            // Очищаем поля формы
            Name.Text = string.Empty;
            SalesCount.Text = string.Empty;
            PhoneNumber.Text = string.Empty;
            Email.Text = string.Empty;
            Priority.Text = string.Empty;
            SuccessRate.Text = string.Empty;
            Close(); // Закрываем окно после добавления нового агента
        }

        // Метод для удаления агента
        private void DeleteAgent(object? sender, RoutedEventArgs e)
        {
            if (_editingAgent != null)
            {
                viewModel.Agents.Remove(_editingAgent); // Удаляем агента из коллекции
                Close(); // Закрываем окно после удаления агента
            }
        }

        // Метод, который выполняется при закрытии окна
        private void OnClosed(object? sender, EventArgs e)
        {
            _mainWindow.DataContext = null;
            _mainWindow.DataContext = viewModel; // Обновляем DataContext главного окна
        }
    }
}
