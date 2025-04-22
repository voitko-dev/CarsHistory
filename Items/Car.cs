namespace CarsHistory.Items;

public class Car
{
    public string Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public decimal Price { get; set; }
    public string Comment { get; set; }
    public int Mileage { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime DateAdded { get; set; }
    public int EnginePower { get; set; }
    public string FuelType { get; set; }
    public string Transmission { get; set; }
    public string PhotoUrl { get; set; }
}
