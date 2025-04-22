using System.Windows;
using CarsHistory.Extentions;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class DeleteCarWindow : Window
{
    private readonly FirebaseService firebaseService;

    public DeleteCarWindow()
    {
        InitializeComponent();
        firebaseService = FirebaseService.GetInstance();
    }

    // Обробник події для кнопки видалення
    private async void btnDeleteCars_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;
            
            if (!startDate.HasValue || !endDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }
            
            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be later than end date.");
                return;
            }
            
            List<Car> cars = await firebaseService.GetCarsAsync();

            // Фільтрація за датою додавання
            List<Car> carsToDelete = new List<Car>();
            foreach (Car car in cars)
            {
                if (car.DateAdded >= startDate.Value.ToUtcSafe() && car.DateAdded <= endDate.Value.ToUtcSafe())
                    carsToDelete.Add(car);
            }
            
            if (carsToDelete.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + carsToDelete.Count + " cars?", "Delete Confirmation", MessageBoxButton.YesNo);
                
                if (result == MessageBoxResult.Yes)
                {
                    foreach (Car car in carsToDelete)
                    {
                        await firebaseService.DeleteCarAsync(car.Id);
                    }

                    MessageBox.Show("Cars deleted successfully!");
                    txtStatus.Text = carsToDelete.Count + " cars deleted successfully.";
                }
                else
                {
                    txtStatus.Text = "Car deletion cancelled.";
                }
            }
            else
            {
                MessageBox.Show("No cars found in the selected date range.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}