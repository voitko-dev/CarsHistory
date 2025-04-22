using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class CarStatusWindow : Window
    {
        public CarStatusWindow()
        {
            InitializeComponent();
            LoadCarStatusData();
            DisableSaveButton();
            DisableDeleteButton();

            if (FirebaseService.GetInstance().CurUserRole == UsersRole.Admin)
                btnDeleteAll.Visibility = Visibility.Visible;
        }

        private async void LoadCarStatusData()
        {
            try
            {
                FirebaseService firebase = FirebaseService.GetInstance();

                List<Car> carsList = await firebase.GetCarsWithStatusAsync();
                List<CarStatus> carStatusList = await firebase.GetCarStatusesAsync();

                List<string> validCarIds = carStatusList.Select(status => status.CarId).ToList();
                List<Car> filteredCars = carsList.Where(car => validCarIds.Contains(car.Id)).ToList();

                List<DisplayCarStatus> displayList = new List<DisplayCarStatus>();

                int countProcessWithoutCarData = 0;

                foreach (CarStatus carStatus in carStatusList)
                {
                    Car? curCar = filteredCars.FirstOrDefault(t => t.Id == carStatus.CarId);
                    if (curCar != null)
                        displayList.Add(new DisplayCarStatus(carStatus, curCar));
                    else
                        countProcessWithoutCarData++;
                }

                dataGridCarStatus.ItemsSource = displayList;

                if (countProcessWithoutCarData > 0)
                    MessageBox.Show("Error: some process without car data");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading car statuses: " + ex.Message);
            }
        }

        #region BtnsMethods

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void btnUserColors_Click(object sender, RoutedEventArgs e)
        {
            UserColorsWindow userColorsWindow = new UserColorsWindow();
            userColorsWindow.Show();
        }

        private async Task DeleteSelectedItems()
        {
            if (dataGridCarStatus.ItemsSource == null) 
                return;

            List<string> selectedItems = dataGridCarStatus.ItemsSource.Cast<DisplayCarStatus>()
                .Where(item => item.IsSelectedForDeletion)
                .Select(t => t.Id)
                .ToList();

            await DeleteItems(selectedItems);
        }

        private async Task DeleteItems(List<string> data)
        {
            FirebaseService firebase = FirebaseService.GetInstance();

            if (data.Count != 0)
            {
                foreach (string item in data)
                {
                    await firebase.DeleteCarStatusAsync(item);
                }
                LoadCarStatusData(); // Refresh data
                MessageBox.Show("Selected records have been deleted.");
            }
            else
            {
                MessageBox.Show("No records selected for deletion.");
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllItems();
        }

        private void BtnReloadData_Click(object sender, RoutedEventArgs e)
        {
            LoadCarStatusData();

            ToolTip tooltip = new ToolTip
            {
                Content = new TextBlock
                {
                    Text = "Window reloaded",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                StaysOpen = false,
                Placement = PlacementMode.Mouse,
                Height = 100,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 200,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181735")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C70"))
            };

            tooltip.IsOpen = true;

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, args) =>
            {
                tooltip.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        private async Task DeleteAllItems()
        {
            FirebaseService firebase = FirebaseService.GetInstance();

            if (firebase.CurUserRole != UsersRole.Admin)
            {
                MessageBox.Show("All records can only be deleted by an admin.");
                return;
            }

            List<CarStatus> allData = await firebase.GetCarStatusesAsync();
            if (allData.Any())
            {
                await DeleteItems(allData.Select(t => t.Id).ToList());
                 MessageBox.Show("All records have been deleted.");
            }
            else
            {
                MessageBox.Show("No records to delete.");
            }
        }

        private void btnAddCarStatus_Click(object sender, RoutedEventArgs e)
        {
            AddCarStatusWindow addCarStatusWindow = new AddCarStatusWindow();
            addCarStatusWindow.ShowDialog(); // ShowDialog might be better if you need to wait for it
            LoadCarStatusData(); // Refresh data after add window is closed
        }

        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            List<DisplayCarStatus>? updatedCarStatusList = dataGridCarStatus.ItemsSource as List<DisplayCarStatus>;
            if (updatedCarStatusList == null || updatedCarStatusList.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            try
            {
                string currentUserName = FirebaseService.GetInstance().CurUserName;
                List<CarStatus> result = updatedCarStatusList
                    .Select(displayCarStatus => displayCarStatus.GetCarStatus(currentUserName))
                    .ToList();

                await FirebaseService.GetInstance().SetNewCarStatusDataAsync(result);

                LoadCarStatusData(); // Refresh data

                MessageBox.Show("Data saved successfully!");
                DisableSaveButton(); // Disable save button after successful save
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void DisableSaveButton()
        {
            btnSaveChanges.IsEnabled = false;
            btnSaveChanges.Opacity = 0.5;
        }

        private void EnableSaveButton()
        {
            btnSaveChanges.IsEnabled = true;
            btnSaveChanges.Opacity = 1;
        }

        private void DisableDeleteButton()
        {
            btnDelete.IsEnabled = false;
            btnDelete.Opacity = 0.5;
        }

        private void EnableDeleteButton()
        {
            btnDelete.IsEnabled = true;
            btnDelete.Opacity = 1;
        }

        #endregion

        private void dataGridCarStatus_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.Item is DisplayCarStatus displayCarStatus)
                {
                    if (e.Column is DataGridBoundColumn column)
                    {
                        var bindingPath = (column.Binding as Binding)?.Path.Path;
                        if (!string.IsNullOrEmpty(bindingPath) && bindingPath.EndsWith(".fieldValue"))
                        {
                            string propertyName = bindingPath.Substring(0, bindingPath.LastIndexOf(".fieldValue", StringComparison.Ordinal));
                            var propertyInfo = typeof(DisplayCarStatus).GetProperty(propertyName);

                            if (propertyInfo != null && propertyInfo.PropertyType == typeof(FieldWithAuthor<DateTime?>))
                            {
                                if (propertyInfo.GetValue(displayCarStatus) is FieldWithAuthor<DateTime?> fieldWithAuthor)
                                {
                                    fieldWithAuthor.lastPersonChange = FirebaseService.GetInstance().CurUserName;
                                }
                            }
                        }
                    }
                    if (!btnSaveChanges.IsEnabled)
                        EnableSaveButton();
                }
            }
            
            if (e.Row.Item is DisplayCarStatus editedItem)
            {
                if (e.Column is DataGridCheckBoxColumn checkBoxColumn && (checkBoxColumn.Binding as Binding)?.Path.Path == nameof(DisplayCarStatus.IsSelectedForDeletion))
                 {
                    if (dataGridCarStatus.ItemsSource.Cast<DisplayCarStatus>().Any(item => item.IsSelectedForDeletion))
                        EnableDeleteButton();
                    else
                        DisableDeleteButton();
                 }
            }
        }
    }
}