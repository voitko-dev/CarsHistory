using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class AddCarStatusWindow : Window
{
    public AddCarStatusWindow()
    {
        InitializeComponent();
        _ = SetDisplayData();
    }

    private async Task SetDisplayData()
    {
        FirebaseService firebase = FirebaseService.GetInstance();
        List<Car> carsList = await firebase.GetCarsWithoutStatusAsync();
        Dictionary<string, string> formatedData = new Dictionary<string, string>();

        foreach (Car car in carsList.Where(t => t.Purchased))
        {
            formatedData.TryAdd(car.Id, $"{car.Brand} - {car.Model} - {car.Mileage} - {car.Price} - {car.ProductionDate.ToShortDateString()}");
        }

        if (formatedData.Count == 0)
        {
            MessageBox.Show("First add car data!");
            
            Close();
        }
        
        cmbItemsBox.ItemsSource = formatedData;
    }

    private async void btnSaveStatus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CarStatus carStatus = new CarStatus
            {
                CarId = ((KeyValuePair<string, string>)cmbItemsBox.SelectedItem).Key,
                Auction = txtAuction.Text
            };
            FirebaseService firebase = FirebaseService.GetInstance();
            
            await firebase.AddCarStatusAsync(carStatus);

            List<CarStatus> list = await firebase.GetCarStatusesAsync();

            string? statusId = list.FirstOrDefault(t => t.CarId == carStatus.CarId)?.Id;
            if (statusId != null)
            {
                await firebase.SetCarStatusFieldInCarDataAsync(carStatus.CarId, statusId);
                MessageBox.Show("Car Status added successfully!");
            }
            else
                MessageBox.Show("Try again later!");
            
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}
