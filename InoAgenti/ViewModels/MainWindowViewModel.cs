using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using InoAgenti.Models;

namespace InoAgenti.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Поле для хранения всех агентов, используемых для фильтрации и поиска
        private ObservableCollection<Agent> _allAgents;

        // Свойство для привязки к отображаемому списку агентов
        public ObservableCollection<Agent> Agents { get; private set; }

        // Конструктор, инициализирующий коллекцию агентов
        public MainWindowViewModel()
        {
            // Инициализация списка всех агентов
            _allAgents = new ObservableCollection<Agent>
            {
                new Agent
                {
                    Type = "Плохой",
                    Name = "Наименование агента 1",
                    SalesCount = 10,
                    PhoneNumber = "+7 111 111 11 11",
                    Email = "agent1@example.com",
                    Priority = 10,
                    SuccessRate = 10.0,
                    ImagePath = new Bitmap("C:\\Users\\glkho\\OneDrive\\Рабочий стол\\Сессия\\InoAgenti\\InoAgenti\\Assets\\picture.png")
                },
                new Agent
                {
                    Type = "Хороший",
                    Name = "Наименование агента 2",
                    SalesCount = 15,
                    PhoneNumber = "+7 222 222 22 22",
                    Email = "agent2@example.com",
                    Priority = 8,
                    SuccessRate = 15.0,
                    ImagePath = new Bitmap("C:\\Users\\glkho\\OneDrive\\Рабочий стол\\Сессия\\InoAgenti\\InoAgenti\\Assets\\picture.png")
                }
            };

            // Копирование всех агентов в отображаемую коллекцию
            Agents = new ObservableCollection<Agent>(_allAgents);
        }

        // Метод для фильтрации агентов по типу
        public ObservableCollection<Agent> filtration(string selectedType)
        {
            // Если выбранный тип пустой или равен "Все типы", возвращаем всех агентов
            var filteredAgents = string.IsNullOrEmpty(selectedType) || selectedType == "Все типы"
                ? new ObservableCollection<Agent>(_allAgents)
                : new ObservableCollection<Agent>(_allAgents.Where(agent => agent.Type == selectedType));

            // Обновление коллекции отображаемых агентов
            UpdateAgents(filteredAgents);
            return Agents;
        }

        // Метод для сортировки агентов
        public ObservableCollection<Agent> Sort(string sortOption)
        {
            // Сортировка агентов в зависимости от выбранной опции
            IOrderedEnumerable<Agent> sortedAgents = sortOption switch
            {
                "От А до Я наименование" => Agents.OrderBy(a => a.Name),
                "От Я до А наименование" => Agents.OrderByDescending(a => a.Name),
                "Скидка по возрастанию" => Agents.OrderBy(a => a.SalesCount),
                "Скидка по убыванию" => Agents.OrderByDescending(a => a.SalesCount),
                "Приоритет по возрастанию" => Agents.OrderBy(a => a.Priority),
                "Приоритет по убыванию" => Agents.OrderByDescending(a => a.Priority),
                _ => Agents.OrderBy(a => a.Name),
            };

            // Обновление коллекции отображаемых агентов
            Agents = new ObservableCollection<Agent>(sortedAgents);
            return Agents;
        }

        // Метод для поиска агентов по имени, номеру телефона или email
        public ObservableCollection<Agent> Search(string searchText)
        {
            // Поиск агентов, если строка поиска не пустая или null
            var searchedAgents = string.IsNullOrWhiteSpace(searchText)
                ? new ObservableCollection<Agent>(_allAgents)
                : new ObservableCollection<Agent>(_allAgents.Where(agent =>
                    agent.Name.ToLower().Contains(searchText.ToLower()) ||
                    agent.PhoneNumber.Contains(searchText.ToLower()) ||
                    (agent.Email != null && agent.Email.ToLower().Contains(searchText.ToLower()))
                ));

            // Обновление коллекции отображаемых агентов
            UpdateAgents(searchedAgents);
            return Agents;
        }

        // Метод для добавления нового агента
        public void AddAgent(Agent newAgent)
        {
            // Добавление агента в обе коллекции
            _allAgents.Add(newAgent);
            Agents.Add(newAgent);
        }

        // Вспомогательный метод для обновления коллекции отображаемых агентов
        public void UpdateAgents(ObservableCollection<Agent> updatedAgents)
        {
            // Очистка текущей коллекции и добавление новых агентов
            Agents.Clear();
            foreach (var agent in updatedAgents)
            {
                Agents.Add(agent);
            }
        }
    }
}