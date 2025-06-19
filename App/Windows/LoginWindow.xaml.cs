using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadSavedEmails();
        }

        public LoginWindow(string email)
        {
            InitializeComponent();

            EmailComboBox.Text = email;
        }

        private void LoadSavedEmails()
        {
            List<string> emails = EmailSaverService.GetInstance().LoadEmailList();
            foreach (string email in emails)
            {
                EmailComboBox.Items.Add(email);
            }

            if (EmailComboBox.Items.Count > 0)
                EmailComboBox.SelectedIndex = 0;
        }

        private void SaveEmail(string email)
        {
            List<string> emails = EmailSaverService.GetInstance().LoadEmailList();
            if (!emails.Contains(email))
                emails.Add(email);

            EmailSaverService.GetInstance().SaveEmailList(emails);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailComboBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Please fill in all fields.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var (userId, isBlocked) = await FirebaseService.SignInUserAsync(email, password);
                
                if (isBlocked)
                {
                    // Користувач заблокований, показуємо повідомлення
                    ShowBlockedUserWindow();
                    return;
                }
                
                if (userId == null)
                    throw new Exception("Invalid username or password.");

                UsersRole role = await FirebaseService.GetUserRoleAsync(userId);

                if (role == UsersRole.Pending)
                {
                    ErrorTextBlock.Text = "Contact the administrator to confirm registration.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                // Зберігаємо email після успішного логіну
                SaveEmail(email);

                MainWindow mainWindow = new MainWindow(userId);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Error: " + ex.Message;
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }
        
        private void ShowBlockedUserWindow()
        {
            BlockedUserWindow blockedWindow = new BlockedUserWindow();
            Close();
            blockedWindow.ShowDialog();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }
    }
}