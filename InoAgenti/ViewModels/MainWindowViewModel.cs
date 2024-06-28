using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using InoAgenti.Models;

namespace InoAgenti.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int _pageSize = 10;
        public ObservableCollection<Agent> _allAgents;
        public ObservableCollection<Agent> _agents { get; }
        public ObservableCollection<int> _pageNumbers { get; private set; }
        private int _currentPageIndex;

        public MainWindowViewModel()
        {
            _allAgents = new ObservableCollection<Agent>
            {
                new Agent
                {
                    Type = "Плохой", Name = "Наименование агента 1", SalesCount = 2000, PhoneNumber = "+7 111 111 11 11", Email = "agent1@example.com", Priority = 10, DiscountRate = 0,
                    ImagePath = new Bitmap("C:\\Users\\glkho\\RiderProjects\\Agents\\InoAgenti\\Assets\\picture.png")
                },
                new Agent
                {
                    Type = "Хороший", Name = "Наименование агента 2", SalesCount = 15, PhoneNumber = "+7 222 222 22 22", Email = "agent2@example.com", Priority = 8,DiscountRate = 0,
                    ImagePath = new Bitmap("C:\\Users\\glkho\\RiderProjects\\Agents\\InoAgenti\\Assets\\picture.png")
                },
            };
            _agents = new ObservableCollection<Agent>(_allAgents.Take(_pageSize));
            _currentPageIndex = 0;
            UpdatePageNumbers();
        }

        private void UpdatePageNumbers()
        {
            int pageCount = (int)Math.Ceiling((double)_allAgents.Count / _pageSize);
            _pageNumbers = new ObservableCollection<int>(Enumerable.Range(1, pageCount));
        }

        private void UpdateAgentsForCurrentPage()
        {
            int startIndex = _currentPageIndex * _pageSize;
            _agents.Clear();

            for (int i = startIndex; i < startIndex + _pageSize && i < _allAgents.Count; i++)
            {
                _agents.Add(_allAgents[i]);
            }
        }

        public void PageNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is int selectedPage)
            {
                _currentPageIndex = selectedPage - 1;
                UpdateAgentsForCurrentPage();
            }
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                UpdateAgentsForCurrentPage();
            }
        }

        public void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int maxPageIndex = _pageNumbers.Count - 1;
            if (_currentPageIndex < maxPageIndex)
            {
                _currentPageIndex++;
                UpdateAgentsForCurrentPage();
            }
        }

        public void filtration(string selectedType)
        {
            if (selectedType == "Все типы")
            {
                UpdateAgents(new ObservableCollection<Agent>(_allAgents));
            }
            else
            {
                var filteredAgents =
                    new ObservableCollection<Agent>(_allAgents.Where(agent => agent.Type == selectedType));
                UpdateAgents(filteredAgents);
            }
        }

        public void Sort(string sortOption)
        {
            if (sortOption == "Сортировка")
            {
                UpdateAgents(new ObservableCollection<Agent>(_allAgents));
            }
            else
            {
                IOrderedEnumerable<Agent> sortedAgents = sortOption switch
                {
                    "От А до Я наименование" => _agents.OrderBy(a => a.Name),
                    "От Я до А наименование" => _agents.OrderByDescending(a => a.Name),
                    "Скидка по возрастанию" => _agents.OrderBy(a => a.SalesCount),
                    "Скидка по убыванию" => _agents.OrderByDescending(a => a.SalesCount),
                    "Приоритет по возрастанию" => _agents.OrderBy(a => a.Priority),
                    "Приоритет по убыванию" => _agents.OrderByDescending(a => a.Priority),
                    _ => _agents.OrderBy(a => a.Name),
                };

                UpdateAgents(new ObservableCollection<Agent>(sortedAgents));
            }
        }

        public void Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateAgents(new ObservableCollection<Agent>(_allAgents));
            }
            else
            {
                var searchedAgents = new ObservableCollection<Agent>(_allAgents.Where(agent =>
                    agent.Name.ToLower().Contains(searchText.ToLower()) ||
                    agent.PhoneNumber.Contains(searchText.ToLower()) ||
                    (agent.Email != null && agent.Email.ToLower().Contains(searchText.ToLower()))
                ));
                UpdateAgents(searchedAgents);
            }
        }

        public void AddAgent(Agent newAgent)
        {
            _allAgents.Add(newAgent);
            _agents.Add(newAgent);
        }

        public void UpdateAgents(ObservableCollection<Agent> updatedAgents)
        {
            _agents.Clear();
            foreach (var agent in updatedAgents)
            {
                _agents.Add(agent);
            }
        }
    }
}