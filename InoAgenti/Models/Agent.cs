using Avalonia.Media.Imaging;

namespace InoAgenti.Models;

public class Agent 
{
    public string Type { get; set; }
    public string Name { get; set; }
    public int SalesCount { get; set; }
    public string PhoneNumber { get; set; }
    public int Priority { get; set; }
    public double SuccessRate { get; set; }

    public string Email { get; set; } 
    public Bitmap ImagePath { get; set; }
}