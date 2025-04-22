using System.Windows;
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
            // Отримуємо діапазон дат
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            // Перевірка на правильність введених дат
            if (!startDate.HasValue || !endDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }

            // Перевірка, чи правильний діапазон дат (початкова дата не повинна бути пізніше кінцевої)
            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be later than end date.");
                return;
            }

            // Отримуємо всі автомобілі з Firebase
            var cars = await firebaseService.GetCarsAsync();

            // Фільтрація за датою додавання
            var carsToDelete = new List<Car>();
            foreach (var car in cars)
            {
                if (car.DateAdded >= startDate.Value && car.DateAdded <= endDate.Value)
                {
                    carsToDelete.Add(car);
                }
            }

            // Якщо знайдені автомобілі для видалення
            if (carsToDelete.Count > 0)
            {
                // Підтвердження видалення
                var result = MessageBox.Show("Are you sure you want to delete " + carsToDelete.Count + " cars?",
                    "Delete Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Видаляємо автомобілі
                    foreach (var car in carsToDelete)
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