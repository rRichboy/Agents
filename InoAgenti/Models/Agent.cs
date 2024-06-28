using Avalonia.Media.Imaging;

namespace InoAgenti.Models;

public class Agent
{
    public string Type { get; set; }
    public string Name { get; set; }
    public int SalesCount { get; set; } 
    public string PhoneNumber { get; set; }
    public int Priority { get; set; }
    public string Email { get; set; }
    public Bitmap ImagePath { get; set; }
    public string Address { get; set; }
    public string INN { get; set; }
    public string KPP { get; set; }
    public string DirectorName { get; set; }
    public double DiscountRate { get; set; }

    public void CalculateDiscount()
    {
        double totalSales = SalesCount;
        if (totalSales < 10000)
            DiscountRate = 0;
        else if (totalSales <= 50000)
            DiscountRate = 5;
        else if (totalSales <= 150000)
            DiscountRate = 10;
        else if (totalSales <= 500000)
            DiscountRate = 20;
        else
            DiscountRate = 25;

        if (DiscountRate == 25)
        {
            
        }
        
    }
}