using System.Windows;
using System.Windows.Controls;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class RegisterWindow : Window
    {
        private bool _canSetRole = false;
        public RegisterWindow()
        {
            InitializeComponent();
            EmailTextBox.TextChanged += (_, _) => { ShowRolesButton.Visibility = Visibility.Collapsed; };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        
        private void ShowRolesButton_Click(object sender, RoutedEventArgs e)
        {
            _canSetRole = true;
            RoleComboBox.Visibility = Visibility.Visible;
            ShowRolesButton.Visibility = Visibility.Collapsed;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string? password = PasswordBox.Password;
            string name = NameTextBox.Text;
            string? role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                ErrorTextBlock.Text = "Please fill in all fields.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                if (_canSetRole)
                    await FirebaseService.RegisterUserAsync(email, password, role, name);
                else
                    await FirebaseService.RegisterUserWithApproveAsync(email, password, name);
                
                MessageBox.Show("User registered successfully!");
                
                LoginWindow loginWindow = new LoginWindow(email);
                loginWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Error: " + ex.Message;
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}