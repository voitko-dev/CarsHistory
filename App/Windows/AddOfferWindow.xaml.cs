using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class AddOfferWindow : Window
    {
        private readonly FirebaseService _firebaseService;
        public OfferItem NewOffer { get; private set; }

        // Конструктор тепер приймає список авто для вибору
        public AddOfferWindow(Dictionary<string, string> carSearchItems)
        {
            InitializeComponent();
            _firebaseService = FirebaseService.GetInstance();

            // Заповнюємо випадаючий список авто
            cmbCarSearch.ItemsSource = carSearchItems;
            if (carSearchItems.Any())
            {
                cmbCarSearch.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCarSearch.SelectedItem == null || string.IsNullOrWhiteSpace(txtLink.Text) || dpEndDate.SelectedDate == null || txtTime.Value == null)
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime datePart = dpEndDate.SelectedDate.Value;
            TimeSpan timePart = txtTime.Value.Value.TimeOfDay;
            DateTime combinedDateTime = datePart.Add(timePart);

            double.TryParse(txtMaxPrice.Text, out double maxPrice);

            var selectedCarSearch = (KeyValuePair<string, string>)cmbCarSearch.SelectedItem;

            NewOffer = new OfferItem
            {
                CarSearchItemId = selectedCarSearch.Key,
                GroupName = ((KeyValuePair<string, string>)cmbCarSearch.SelectedItem).Value.Split('(')[1].TrimEnd(')'),
                Link = txtLink.Text,
                Description = txtDescription.Text,
                EndDate = combinedDateTime.ToUniversalTime(),
                MaxPrice = maxPrice,
                Status = OfferStatus.NotSelected,
                LastChangeAuthor = _firebaseService.CurUserName
            };

            DialogResult = true;
            Close();
        }
    }
}