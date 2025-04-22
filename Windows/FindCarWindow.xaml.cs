using System.Windows;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class FindCarWindow : Window
{
    private readonly FirebaseService firebaseService;

    public FindCarWindow()
    {
        InitializeComponent();
        firebaseService = FirebaseService.GetInstance();
    }

    // Обробка натискання кнопки для пошуку
    private async void btnSearchCar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string searchQuery = txtSearch.Text.Trim().ToLower();
                
            // Якщо не введено нічого для пошуку
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Please enter a brand or model to search.");
                return;
            }

            // Отримання всіх автомобілів з Firebase
            var cars = await firebaseService.GetCarsAsync();
                
            // Фільтрація результатів пошуку
            var filteredCars = cars.Where(car => car.Brand.ToLower().Contains(searchQuery) ||
                                                 car.Model.ToLower().Contains(searchQuery))
                .ToList();

            // Виведення результатів у DataGrid
            dataGridCars.ItemsSource = filteredCars;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}