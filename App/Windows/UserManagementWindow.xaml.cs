using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class UserManagementWindow : Window
{
    private ObservableCollection<User> _users = new ObservableCollection<User>();
    private User? _selectedUser;

    public UserManagementWindow()
    {
        InitializeComponent();
        UsersDataGrid.ItemsSource = _users;
        LoadUsers();
    }

    private async void LoadUsers()
    {
        try
        {
            var users = await FirebaseService.GetAllUsersAsync();

            _users.Clear();
            foreach (var user in users)
            {
                // Не показуємо поточного користувача в списку
                if (user.Id != FirebaseService.CurrentUser?.Id && user.Role != UsersRole.SuperAdmin)
                {
                    _users.Add(user);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження користувачів: {ex.Message}", "Помилка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void btnReloadUsers_Click(object sender, RoutedEventArgs e)
    {
        LoadUsers();
    }
    
    private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedUser = UsersDataGrid.SelectedItem as User;
    }

    private async void BlockButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string userId)
        {
            try
            {
                // Знаходимо користувача в колекції за Id
                User? userToUpdate = null;
                foreach (var user in _users)
                {
                    if (user.Id == userId)
                    {
                        userToUpdate = user;
                        break;
                    }
                }

                if (userToUpdate != null)
                {
                    // Інвертуємо статус блокування
                    bool newBlockedStatus = !userToUpdate.IsBlocked;

                    // Запитуємо підтвердження
                    string action = newBlockedStatus ? "заблокувати" : "розблокувати";
                    string message = $"Ви впевнені, що хочете {action} користувача {userToUpdate.Name}?";

                    MessageBoxResult result = MessageBox.Show(message, "Підтвердження",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (userToUpdate.Role == UsersRole.SuperAdmin)
                            return;
                        
                        // Оновлюємо статус в базі даних
                        await FirebaseService.BlockUserAsync(userId, newBlockedStatus);

                        // Оновлюємо статус в моделі
                        userToUpdate.IsBlocked = newBlockedStatus;

                        // Оновлюємо відображення
                        UsersDataGrid.Items.Refresh();

                        string statusMessage = newBlockedStatus
                            ? "Користувача успішно заблоковано"
                            : "Користувача успішно розблоковано";

                        MessageBox.Show(statusMessage, "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}