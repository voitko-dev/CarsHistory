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
    public partial class CarSearchWindow : Window
    {
        private FirebaseService firebase = null;
        
        public CarSearchWindow()
        {
            InitializeComponent();
            LoadCarSearchData();
            DisableSaveButton();
            DisableDeleteButton();
            
            bool isAdmin = firebase.CurUserRole == UsersRole.Admin;
            
            btnDeleteAll.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnClearDates.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            
            if (!isAdmin)
            {
                var deleteColumn = dataGridCarSearch.Columns.FirstOrDefault(
                    c => c is DataGridCheckBoxColumn checkBoxColumn && 
                         (checkBoxColumn.Binding as Binding)?.Path.Path == nameof(DisplayCarSearch.IsSelectedForDeletion));
        
                if (deleteColumn != null)
                {
                    deleteColumn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void LoadCarSearchData()
        {
            try
            {
                firebase = FirebaseService.GetInstance();

                List<CarSearchItem> carSearchList = await firebase.GetCarsSearchAsync();

                List<DisplayCarSearch> displayList = new List<DisplayCarSearch>();

                foreach (CarSearchItem carSearch in carSearchList)
                {
                    displayList.Add(new DisplayCarSearch(carSearch));
                }

                dataGridCarSearch.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних пошуку авто: " + ex.Message);
            }
        }

        #region BtnsMethods

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }
        
        private async void btnClearDates_Click(object sender, RoutedEventArgs e)
        {
            if (firebase.CurUserRole != UsersRole.Admin)
            {
                MessageBox.Show("Операція доступна тільки для адміністраторів.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Ви впевнені, що хочете очистити всі дати аукціонів?\nЦя дія незворотна!",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await firebase.ClearAuctionDatesAsync();
                    MessageBox.Show("Дати аукціонів успішно очищено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCarSearchData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при очищенні дат: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private async Task DeleteSelectedItems()
        {
            if (dataGridCarSearch.ItemsSource == null) 
                return;
            
            if (firebase.CurUserRole != UsersRole.Admin)
            {
                MessageBox.Show("Записи можуть бути видалені тільки адміністратором.");
                return;
            }

            List<string> selectedItems = dataGridCarSearch.ItemsSource.Cast<DisplayCarSearch>()
                .Where(item => item.IsSelectedForDeletion)
                .Select(t => t.Id)
                .ToList();

            await DeleteItems(selectedItems);
        }

        private async Task DeleteItems(List<string> data)
        {
            if (data.Count != 0)
            {
                foreach (string item in data)
                {
                    await firebase.DeleteCarSearchAsync(item);
                }
                LoadCarSearchData();
                MessageBox.Show("Вибрані записи видалено.");
                
                LoadCarSearchData();
            }
            else
            {
                MessageBox.Show("Немає вибраних записів для видалення.");
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllItems();
        }

        private void BtnReloadData_Click(object sender, RoutedEventArgs e)
        {
            LoadCarSearchData();

            ToolTip tooltip = new ToolTip
            {
                Content = new TextBlock
                {
                    Text = "Вікно перезавантажено",
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
            if (firebase.CurUserRole != UsersRole.Admin)
            {
                MessageBox.Show("Всі записи можуть бути видалені тільки адміністратором.");
                return;
            }

            List<CarSearchItem> allData = await firebase.GetCarsSearchAsync();
            if (allData.Any())
            {
                await DeleteItems(allData.Select(t => t.Id).ToList());
                MessageBox.Show("Всі записи видалено.");
                LoadCarSearchData();
            }
            else
            {
                MessageBox.Show("Немає записів для видалення.");
            }
        }

        private void btnAddCarSearch_Click(object sender, RoutedEventArgs e)
        {
            AddCarSearchWindow addCarSearchWindow = new AddCarSearchWindow();
            addCarSearchWindow.ShowDialog();
            LoadCarSearchData();
        }

        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            List<DisplayCarSearch>? updatedCarSearchList = dataGridCarSearch.ItemsSource as List<DisplayCarSearch>;
            if (updatedCarSearchList == null || updatedCarSearchList.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            try
            {
                string currentUserName = firebase.CurUserName;
                List<CarSearchItem> result = updatedCarSearchList
                    .Select(displayCarSearch => displayCarSearch.GetCarSearch(currentUserName))
                    .ToList();

                await FirebaseService.GetInstance().SetNewCarSearchDataAsync(result);

                LoadCarSearchData(); // Refresh data

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
        
        private void btnUserColors_Click(object sender, RoutedEventArgs e)
        {
            UserColorsWindow userColorsWindow = new UserColorsWindow();
            userColorsWindow.Show();
        }
        #endregion

        private void dataGridCarSearch_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.Item is DisplayCarSearch)
                {
                    if (!btnSaveChanges.IsEnabled)
                        EnableSaveButton();
                }
            }
            
            if (e.Row.Item is DisplayCarSearch _)
            {
                if (e.Column is DataGridCheckBoxColumn checkBoxColumn && (checkBoxColumn.Binding as Binding)?.Path.Path == nameof(DisplayCarSearch.IsSelectedForDeletion))
                {
                    if (dataGridCarSearch.ItemsSource.Cast<DisplayCarSearch>().Any(item => item.IsSelectedForDeletion))
                        EnableDeleteButton();
                    else
                        DisableDeleteButton();
                }
            }
        }
    }
}