using System.Windows;
using System.Windows.Controls;
using CarsHistory.Extentions;
using CarsHistory.Items;
using CarsHistory.Services;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace CarsHistory.Windows;

public partial class AddCarWindow : Window
{
    private readonly FirebaseService firebaseService;

    public AddCarWindow()
    {
        InitializeComponent();
        firebaseService = FirebaseService.GetInstance();
    }
    
    private async void btnAddCar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text) || string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Brand and Model are required!");
                return;
            }

            DateTime TryGetUtcTime(DateTimePicker element)
            {
                if (element.Value == null)
                    return DateTime.Today.ToUtcSafe();
                
                DateTime localDateTime = DateTime.SpecifyKind(element.Value.Value, DateTimeKind.Local);
                
                return localDateTime.ToUtcSafe();
            }
            
            Car car = new Car
            {
                Brand = txtBrand.Text,
                Model = txtModel.Text,
                Price = double.TryParse(txtPrice.Text, out double value) ? value : 0,
                Comment = txtComment.Text,
                Mileage = int.TryParse(txtMileage.Text, out int value2) ? value2 : 0,
                ProductionDate = TryGetUtcTime(dtpProductionDate),
                AuctionDate = TryGetUtcTime(dtpAuctionDate),
                DateAdded = DateTime.UtcNow,
                EnginePower = int.TryParse(txtEnginePower.Text, out int value3) ? value3 : 0,
                СustomsСlearanceСosts = double.TryParse(txtCustomsCosts.Text, out double value4) ? value4 : 0,
                VIN = txtVIN.Text,
                FuelType = ((ComboBoxItem)cmbFuelType.SelectedItem).Content.ToString() ?? "None",
                Transmission = ((ComboBoxItem)cmbTransmission.SelectedItem).Content.ToString() ?? "None",
                CarFrom = ((ComboBoxItem)cmbCarFrom.SelectedItem).Content.ToString() ?? "None",
                Author = firebaseService.CurUserName,
                IdAuthor = firebaseService.CurUserId,
                Purchased = cbPurchased.IsChecked ?? false
            };
            
            await firebaseService.AddCarAsync(car);
            MessageBox.Show("Car added successfully!");
            
            ClearAllFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
    
    private void ClearAllFields()
    {
        txtBrand.Clear();
        txtModel.Clear();
        txtPrice.Clear();
        txtComment.Clear();
        txtMileage.Clear();
        txtEnginePower.Clear();
        txtCustomsCosts.Clear();
        txtVIN.Clear();
        
        dtpProductionDate.Value = null;
        dtpAuctionDate.Value = null;

        cmbFuelType.SelectedIndex = 0;
        cmbTransmission.SelectedIndex = 0;
        cmbCarFrom.SelectedIndex = 0;

        cbPurchased.IsChecked = false;
    }
}