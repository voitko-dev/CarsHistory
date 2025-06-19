using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class DeleteCarWindow : Window
    {
        private ObservableCollection<Car> cars;
        private readonly FirebaseService _firebaseService;

        public DeleteCarWindow()
        {
            InitializeComponent();
            _firebaseService = FirebaseService.GetInstance();
            LoadCars();
        }

        private async void LoadCars()
        {
            try
            {
                List<Car> carsList = await _firebaseService.GetCarsAsync();
                cars = new ObservableCollection<Car>(carsList);
                carsDataGrid.ItemsSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні автомобілів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteSingleCar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Car car)
            {
                DeleteCar(car);
            }
        }

        private async void DeleteCar(Car car)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Ви впевнені, що хочете видалити автомобіль {car.Brand} {car.Model}?", 
                    "Підтвердження видалення", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _firebaseService.DeleteCarAsync(car.Id);
                    cars.Remove(car);
                    txtStatus.Text = "Автомобіль успішно видалено";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні автомобіля: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}