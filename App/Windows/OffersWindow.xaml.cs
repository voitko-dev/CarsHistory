using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class OffersWindow : Window
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<OfferItem> Offers { get; set; }

        private List<OfferItem> _allLoadedOffers = new List<OfferItem>();

        private Dictionary<string, string> _currentGroupCarSearches = new Dictionary<string, string>();

        public OffersWindow()
        {
            InitializeComponent();
            _firebaseService = FirebaseService.GetInstance();
            Offers = new ObservableCollection<OfferItem>();
            DataContext = this;

            comboStatusColumn.ItemsSource = Enum.GetValues(typeof(OfferStatus));

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                var groups = await _firebaseService.GetCarSearchGroupsAsync();
                cmbGroupFilter.ItemsSource = groups;
                if (groups.Any())
                {
                    cmbGroupFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження груп: {ex.Message}", "Помилка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void cmbGroupFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGroupFilter.SelectedItem is string selectedGroup)
            {
                await LoadOffersByGroupAsync(selectedGroup);
            }
        }

        private void LinkTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is OfferItem offer)
            {
                Converter.UrlToHyperlinkConverter.ProcessTextForLinks(offer.Link, textBlock);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var offersToDelete = Offers.Where(o => o.IsSelectedForDeletion).ToList();
            if (!offersToDelete.Any())
            {
                MessageBox.Show("Немає обраних записів для видалення.", "Інформація", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Ви впевнені, що хочете видалити {offersToDelete.Count} запис(ів)?",
                "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var offerIdsToDelete = offersToDelete.Select(o => o.Id).ToList();
                    await _firebaseService.DeleteOffersAsync(offerIdsToDelete);
                    MessageBox.Show("Обрані записи успішно видалено.", "Успіх", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    await LoadOffersByGroupAsync(cmbGroupFilter.SelectedItem as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private async Task LoadOffersByGroupAsync(string group)
        {
            btnSaveChanges.IsEnabled = false;

            try
            {
                var carSearchesInGroup = await _firebaseService.GetCarsSearchByGroupAsync(group);
                _currentGroupCarSearches =
                    carSearchesInGroup.ToDictionary(cs => cs.Id, cs => $"{cs.Brand} {cs.Model} ({cs.Group})");

                var carSearchIds = carSearchesInGroup.Select(cs => cs.Id).ToList();
                var allOffersInGroup = await _firebaseService.GetOffersByCarSearchIdsAsync(carSearchIds);

                var offersToUpdate = new List<OfferItem>();
                var offerIdsToDelete = new List<string>();
                var now = DateTime.UtcNow;

                foreach (var offer in allOffersInGroup)
                {
                    if (offer.EndDate.HasValue && offer.EndDate.Value.ToUniversalTime() < now)
                    {
                        if (offer.Status != OfferStatus.NotPlaying)
                        {
                            offer.Status = OfferStatus.NotPlaying;
                            offersToUpdate.Add(offer);
                        }

                        if ((now - offer.EndDate.Value.ToUniversalTime()).TotalDays > 1)
                        {
                            offerIdsToDelete.Add(offer.Id);
                        }
                    }
                }

                if (offersToUpdate.Any())
                    await _firebaseService.UpdateOffersAsync(offersToUpdate);

                if (offerIdsToDelete.Any())
                    await _firebaseService.DeleteOffersAsync(offerIdsToDelete);

                if (offersToUpdate.Any() || offerIdsToDelete.Any())
                    _allLoadedOffers = await _firebaseService.GetOffersByCarSearchIdsAsync(carSearchIds);

                else
                    _allLoadedOffers = allOffersInGroup;

                foreach (var offer in _allLoadedOffers)
                {
                    var parentCarSearch = carSearchesInGroup.FirstOrDefault(cs => cs.Id == offer.CarSearchItemId);
                    if (parentCarSearch != null)
                    {
                        offer.GroupName = parentCarSearch.Group;
                    }
                }

                ApplyFiltersAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження оферів: {ex.Message}", "Помилка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void chkShowOnlyPlaying_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            Offers.Clear();

            IEnumerable<OfferItem> filteredOffers = _allLoadedOffers
                .Where(o => o.EndDate.HasValue &&
                            o.EndDate.Value.ToUniversalTime() > DateTime.UtcNow);

            if (chkShowOnlyPlaying.IsChecked == true)
            {
                filteredOffers = filteredOffers.Where(o => o.Status == OfferStatus.Playing);
            }

            var sortedOffers = filteredOffers.OrderBy(o => o.EndDate);

            foreach (var offer in sortedOffers)
            {
                offer.ResetModified();
                Offers.Add(offer);
            }
        }

        private async void btnAddOffer_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentGroupCarSearches.Any())
            {
                MessageBox.Show("Спочатку оберіть групу, в якій є авто, щоб додати офер.", "Увага", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            AddOfferWindow addWindow = new AddOfferWindow(_currentGroupCarSearches);
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    await _firebaseService.AddOfferAsync(addWindow.NewOffer);
                    MessageBox.Show("Офер успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Оновлюємо список
                    await LoadOffersByGroupAsync(cmbGroupFilter.SelectedItem as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка збереження нового оферу: {ex.Message}", "Помилка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }


        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var modifiedOffers = Offers.Where(o => o.IsModified).ToList();
            if (!modifiedOffers.Any())
            {
                MessageBox.Show("Немає змін для збереження.", "Інформація", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                await _firebaseService.UpdateOffersAsync(modifiedOffers);

                foreach (var offer in modifiedOffers)
                {
                    offer.ResetModified();
                }

                btnSaveChanges.IsEnabled = false;
                MessageBox.Show("Зміни успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void dataGridOffers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is OfferItem item)
            {
                item.LastChangeAuthor = _firebaseService.CurUserName;
                if (!btnSaveChanges.IsEnabled)
                {
                    btnSaveChanges.IsEnabled = true;
                }
            }
        }

        private void dataGridOffers_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                var row = FindVisualParent<DataGridRow>(source);
                if (row == null)
                {
                    dataGridOffers.UnselectAll();
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null)
                return null;

            DependencyObject parentObject = LogicalTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                if (child is FrameworkElement fe)
                {
                    parentObject = fe.Parent;
                    if (parentObject == null)
                        parentObject = VisualTreeHelper.GetParent(child);
                }
                else
                {
                    parentObject = VisualTreeHelper.GetParent(child);
                }
            }

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
    }

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}