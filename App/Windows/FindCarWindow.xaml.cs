using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CarsHistory.Converter;
using CarsHistory.Extentions;
using CarsHistory.Items;
using CarsHistory.Services;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace CarsHistory.Windows;

public partial class FindCarWindow : Window
{
    private readonly FirebaseService firebaseService;
    private ObservableCollection<Car> filteredCars = new();

    public FindCarWindow()
    {
        InitializeComponent();
        firebaseService = FirebaseService.GetInstance();
        setCMBAuthor();

        SetStateBtnDelete();
    }

    private void SetStateBtnDelete()
    {
        if (firebaseService.CurUserRole != UsersRole.Admin)
            btnDeleteCar.Visibility = Visibility.Collapsed;
    }

    private async Task setCMBAuthor()
    {
        List<string> data = await FirebaseService.GetAllUsersNameAsync();
        data.Insert(0, "None");
        cmbAuthorType.ItemsSource = data;
    }
    
    private async void btnDeleteCar_Click(object sender, RoutedEventArgs e)
    {
        if (dataGridCars.SelectedItem is Car selectedCar)
        {
            MessageBoxResult result = MessageBox.Show($"Ви впевнені, що хочете видалити авто {selectedCar.Brand} {selectedCar.Model}?", 
                "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                filteredCars.Remove(selectedCar);
                
                await firebaseService.DeleteCarAsync(selectedCar.Id);
            }
        }
        else
        {
            MessageBox.Show("Будь ласка, виберіть авто для видалення.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CommentTextBlock_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            var dataContext = textBlock.DataContext;
            if (dataContext != null && dataContext is Car car)
            {
                string comment = car.Comment;
                if (!string.IsNullOrEmpty(comment))
                {
                    UrlToHyperlinkConverter.ProcessTextForLinks(comment, textBlock);
                }
            }
        }
    }
    
    private void dataGridCars_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.Column.Header.ToString() == "Коментар")
        {
            // Затримка оновлення для коректного рендерингу
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var row = e.Row;
                var cell = dataGridCars.Columns
                    .Where(c => c.Header.ToString() == "Коментар")
                    .Select(c => dataGridCars.Columns.IndexOf(c))
                    .FirstOrDefault();
                
                if (cell >= 0)
                {
                    DataGridRow gridRow = (DataGridRow)dataGridCars.ItemContainerGenerator.ContainerFromItem(row.Item);
                    if (gridRow != null)
                    {
                        DataGridCell gridCell = dataGridCars.GetCell(gridRow, cell);
                        if (gridCell != null)
                        {
                            if (gridCell.ContentTemplate == null)
                                return;
                            
                            var presenter = gridCell.ContentTemplate.FindName("commentTextBlock", gridCell) as TextBlock;
                            if (presenter != null && row.Item is Car car)
                            {
                                UrlToHyperlinkConverter.ProcessTextForLinks(car.Comment, presenter);
                            }
                        }
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }

    private async void btnSearchCar_Click(object sender, RoutedEventArgs e)
    {
        DateTime? TryGetUtcTime(DateTimePicker element)
        {
            if (element?.Value == null)
                return null;
                
            DateTime localDateTime = DateTime.SpecifyKind(element.Value.Value, DateTimeKind.Local);
                
            return localDateTime.ToUtcSafe();
        }
        
        try
        {
            string brandFilter = txtBrand.Text.Trim().ToLower();
            string modelFilter = txtModel.Text.Trim().ToLower();

            DateTime? dateAddedFrom = TryGetUtcTime(dpDateAddedFrom);
            DateTime? dateAddedTo = TryGetUtcTime(dpDateAddedTo);

            int? mileageFrom = string.IsNullOrEmpty(txtMileageFrom.Text) ? null : int.Parse(txtMileageFrom.Text);
            int? mileageTo = string.IsNullOrEmpty(txtMileageTo.Text) ? null : int.Parse(txtMileageTo.Text);

            double? priceFrom = string.IsNullOrEmpty(txtPriceFrom.Text)
                ? null
                : double.Parse(txtPriceFrom.Text);
            double? priceTo = string.IsNullOrEmpty(txtPriceTo.Text) ? null : double.Parse(txtPriceTo.Text);

            int? productionYearFrom = string.IsNullOrEmpty(txtProductionYearFrom.Text)
                ? null
                : int.Parse(txtProductionYearFrom.Text);
            int? productionYearTo = string.IsNullOrEmpty(txtProductionYearTo.Text)
                ? null
                : int.Parse(txtProductionYearTo.Text);

            int? enginePowerFrom = string.IsNullOrEmpty(txtEnginePowerFrom.Text)
                ? null
                : int.Parse(txtEnginePowerFrom.Text);
            int? enginePowerTo = string.IsNullOrEmpty(txtEnginePowerTo.Text)
                ? null
                : int.Parse(txtEnginePowerTo.Text);

            string? authorData = cmbAuthorType.SelectedItem?.ToString();
            string? author = string.IsNullOrEmpty(authorData) || authorData == "None"
                ? null
                : authorData;
            
            string? transmissionData = ((ComboBoxItem)cmbTransmissionType.SelectedItem)?.Content.ToString();
            string? transmission = string.IsNullOrEmpty(transmissionData) || transmissionData == "None"
                ? null
                : transmissionData;

            string? carFromData = ((ComboBoxItem)cmbCarFrom.SelectedItem)?.Content.ToString();
            string? carFrom = string.IsNullOrEmpty(carFromData) || carFromData == "None"
                ? null
                : carFromData;
            
            if (brandFilter.isNullOrEmpty() && modelFilter.isNullOrEmpty() && !dateAddedFrom.HasValue &&
                !dateAddedTo.HasValue &&
                !mileageFrom.HasValue && !mileageTo.HasValue && !priceFrom.HasValue && !priceTo.HasValue &&
                !productionYearFrom.HasValue && !productionYearTo.HasValue && !enginePowerFrom.HasValue &&
                !enginePowerTo.HasValue && author.isNullOrEmpty() && transmission.isNullOrEmpty() && carFrom.isNullOrEmpty())
            {
                MessageBox.Show("Please enter at least one filter.");
                return;
            }

            List<Car> cars = await firebaseService.GetCarsAsync();

            filteredCars = new ObservableCollection<Car>(cars.Where(car =>
                    (brandFilter.isNullOrEmpty() || car.Brand.Contains(brandFilter, StringComparison.CurrentCultureIgnoreCase)) &&
                    (modelFilter.isNullOrEmpty() || car.Model.Contains(modelFilter, StringComparison.CurrentCultureIgnoreCase)) &&
                    (!dateAddedFrom.HasValue || car.DateAdded >= dateAddedFrom) &&
                    (!dateAddedTo.HasValue || car.DateAdded <= dateAddedTo) &&
                    (!mileageFrom.HasValue || car.Mileage >= mileageFrom) &&
                    (!mileageTo.HasValue || car.Mileage <= mileageTo) &&
                    (!priceFrom.HasValue || car.Price >= priceFrom) &&
                    (!priceTo.HasValue || car.Price <= priceTo) &&
                    (!productionYearFrom.HasValue || car.ProductionDate.Year >= productionYearFrom) &&
                    (!productionYearTo.HasValue || car.ProductionDate.Year <= productionYearTo) &&
                    (!enginePowerFrom.HasValue || car.EnginePower >= enginePowerFrom) &&
                    (!enginePowerTo.HasValue || car.EnginePower <= enginePowerTo) &&
                    (author.isNullOrEmpty() || car.Author.Contains(author, StringComparison.CurrentCultureIgnoreCase)) &&
                    (transmission.isNullOrEmpty() || car.Transmission.Contains(transmission, StringComparison.CurrentCultureIgnoreCase)) &&
                    (carFrom.isNullOrEmpty() || car.CarFrom.Contains(carFrom, StringComparison.CurrentCultureIgnoreCase))
                )
                .ToList());

            dataGridCars.ItemsSource = filteredCars;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
    
    private void dataGridCars_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        Car? selectedCar = e.Row.Item as Car;
        if (selectedCar == null)
        {
            e.Cancel = true;
            return;
        }

        // Блокуємо, якщо user і це не його запис
        if (firebaseService.CurUserRole == UsersRole.User && selectedCar.IdAuthor != firebaseService.CurUserId)
        {
            e.Cancel = true;
        }
    }

    private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
    {
        if (dataGridCars.ItemsSource is ObservableCollection<Car> observableCars)
        {
            var updatedCars = observableCars.ToList();

            foreach (Car car in updatedCars)
            {
                car.ProductionDate = car.ProductionDate.ToUtcSafe(true);
                car.AuctionDate = car.AuctionDate.ToUtcSafe(true);
            }

            await FirebaseService.GetInstance().SetNewCarDataAsync(updatedCars);

            MessageBox.Show("Changes saved.");
        }
        else
        {
            MessageBox.Show("Немає даних для збереження.");
        }
    }
    
    private void btnShowCharts_Click(object sender, RoutedEventArgs e)
    {
        if (filteredCars.Count < 1)
        {
            MessageBox.Show("First find cars");
            return;
        }
        
        ChartsWindow chartsWindow = new ChartsWindow(filteredCars.ToList());
        chartsWindow.Show();
    }
}