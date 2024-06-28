using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace InoAgenti.Views;

public partial class ChangePriorityWindow : Window
{
    public int NewPriority { get; private set; }

    public ChangePriorityWindow(int initialPriority)
    {
        InitializeComponent();
        PriorityTextBox.Text = initialPriority.ToString();
    }

    private void ChangePriority_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(PriorityTextBox.Text, out int newPriority))
        {
            NewPriority = newPriority;
            Close();
        }
    }
}
