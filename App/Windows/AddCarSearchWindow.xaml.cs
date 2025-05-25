using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class AddCarSearchWindow : Window
    {
        private readonly FirebaseService _firebaseService;

        public AddCarSearchWindow()
        {
            InitializeComponent();
            _firebaseService = FirebaseService.GetInstance();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка на пусті поля
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Поле 'Марка' не може бути порожнім.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Поле 'Модель' не може бути порожнім.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Створення нового об'єкта CarSearch
                CarSearchItem newCarSearch = new CarSearchItem
                {
                    Brand = txtBrand.Text.Trim(),
                    Model = txtModel.Text.Trim(),
                    Group = txtGroup?.Text?.Trim() ?? "",
                    Comment = txtComment?.Text?.Trim() ?? "",
                    
                    LastPersonChange = _firebaseService.CurUserName,
                    DateUpdated = DateTime.Now.ToUniversalTime(),
                    CarStatusClosed = false,
                    IsSelectedForDeletion = false
                };
                
                await _firebaseService.AddCarSearchAsync(newCarSearch);

                MessageBox.Show("Пошук авто успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні пошуку авто: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}