using System.Windows;
using System.Windows.Controls;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class AddCarWindow : Window
{
    private readonly FirebaseService firebaseService;

    public AddCarWindow()
    {
        InitializeComponent();
        firebaseService = FirebaseService.GetInstance();
    }

    // Обробник події для додавання автомобіля
    private async void btnAddCar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Перевірка на порожні значення в полях
            if (string.IsNullOrWhiteSpace(txtBrand.Text) || string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Brand and Model are required!");
                return;
            }

            // Створення нового автомобіля з введеними даними
            var car = new Car
            {
                Brand = txtBrand.Text,
                Model = txtModel.Text,
                Price = decimal.Parse(txtPrice.Text),
                Comment = txtComment.Text,
                Mileage = int.Parse(txtMileage.Text),
                ProductionDate = dtpProductionDate?.SelectedDate.Value ?? DateTime.Today, // Для прикладу, поточна дата
                DateAdded = DateTime.Today,
                EnginePower = int.Parse(txtEnginePower.Text),
                FuelType = ((ComboBoxItem)cmbFuelType.SelectedItem).Content.ToString() ?? "None",
                Transmission = ((ComboBoxItem)cmbTransmission.SelectedItem).Content.ToString() ?? "None",
                PhotoUrl = "" // Для простоти
            };
            
            // Додаємо автомобіль у Firebase та отримуємо унікальний ключ (Id)
            var result = await firebaseService.AddCarAsync(car);
            car.Id = result.Key;  // Записуємо отриманий ключ як Id
            
            MessageBox.Show("Car added successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}