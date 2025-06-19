using System.Reflection;
using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class MainWindow : Window
{
    private string userUid;
    private UsersRole role;
    public string AppVersion { get; set; }
    
    public MainWindow()
    {
        InitializeComponent();
        
        DisplayAppVersion();
        TakeColorData();
    }

    private void TakeColorData()
    {
        UsersService.GetInstance().TakeUserData();
    }
    
    public MainWindow(string uid)
    {
        InitializeComponent();
        userUid = uid;
        CheckUserRole();

        DisplayAppVersion();
        TakeColorData();
    }

    private void DisplayAppVersion()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        
        if (version == null)
            return;
        
        AppVersion = $"Версія: {version.Major}.{version.Minor}";
        DataContext = this;
    }
    
    private void btnCarOperations_Click(object sender, RoutedEventArgs e)
    {
        carOperationsPopup.IsOpen = !carOperationsPopup.IsOpen;
        if (role is UsersRole.Admin or UsersRole.Admin) 
            SetItemsVisibility();
    }
    
    private async void CheckUserRole()
    {
        (role, string name) = await FirebaseService.GetUserRoleAndNameAsync(userUid);

        if (role is UsersRole.Admin or UsersRole.SuperAdmin) 
            SetItemsVisibility();

        RoleTextBlock.Text = role == UsersRole.None ? "Unknown Role" : $"Welcome, {role} {name}!";
    }
    
    private void UserManagementButton_Click(object sender, RoutedEventArgs e)
    {
        UserManagementWindow userManagementWindow = new UserManagementWindow();
        userManagementWindow.Owner = this;
        userManagementWindow.ShowDialog();
    }

    private void SetItemsVisibility()
    {
        btnDeleteCar.Visibility = Visibility.Visible;
        btnNewUsers.Visibility = Visibility.Visible;
        btnBlockUsers.Visibility = Visibility.Visible;
    }
    
    private void OpenWindowAndClosePopup<T>(Func<T> windowFactory) where T : Window
    {
        carOperationsPopup.IsOpen = false;
        T window = windowFactory();
        window.Show();
    }

    private void btnAddCar_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new AddCarWindow());
    }

    private void btnCarStatus_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new CarStatusWindow());
    }

    private void btnFindCar_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new FindCarWindow());
    }

    private void btnDeleteCar_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new DeleteCarWindow());
    }
    
    private void btnNewCarSearch_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new CarSearchWindow());
    }

    private void btnNewUsers_Click(object sender, RoutedEventArgs e)
    {
        OpenWindowAndClosePopup(() => new NewUsers());
    }
}