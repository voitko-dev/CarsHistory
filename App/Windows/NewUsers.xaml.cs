using System.Windows;
using System.Windows.Controls;
using CarsHistory.Extentions;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class NewUsers : Window
{
    private List<KeyValuePair<string, string>> _pendingUsersList = new List<KeyValuePair<string, string>>();
    
    public NewUsers()
    {
        InitializeComponent();
        _ = ShowItems();
    }

    private async Task ShowItems()
    {
        _pendingUsersList = await FirebaseService.GetPendingUsersAsync();
        PendingUsersList.ItemsSource = _pendingUsersList.Select(t => t.Key).ToList();
    }
    
    private async void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        string? selectedUser = PendingUsersList.SelectedItem.ToString();
        KeyValuePair<string, string> curUser = _pendingUsersList.FirstOrDefault(t => t.Key == selectedUser);
        string userId = curUser.Value;
        if (!userId.IsNullOrEmpty())
        {
            await FirebaseService.ApproveUserAsync(userId);
            MessageBox.Show($"User {selectedUser} has been approved.");
            _pendingUsersList.Remove(curUser);
            PendingUsersList.ItemsSource = _pendingUsersList;
        }
        else
            MessageBox.Show("Please select a user to approve.");
    }
}