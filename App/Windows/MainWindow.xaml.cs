using System.Reflection;
using System.Windows;
using CarsHistory.Items;
using CarsHistory.Services;

namespace CarsHistory.Windows;

public partial class MainWindow : Window
{
    private string userUid;
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
    
    private async void CheckUserRole()
    {
        (UsersRole role, string name) = await FirebaseService.GetUserRoleAndNameAsync(userUid);

        if (role == UsersRole.Admin) 
            SetItemsVisibility();

        RoleTextBlock.Text = role == UsersRole.None ? "Unknown Role" : $"Welcome, {role} {name}!";
    }

    private void SetItemsVisibility()
    {
        btnDeleteCar.Visibility = Visibility.Visible;
        btnNewUsers.Visibility = Visibility.Visible;
    }
    
    private void btnAddCar_Click(object sender, RoutedEventArgs e)
    {
        AddCarWindow addCarWindow = new AddCarWindow();
        addCarWindow.Show();
    }
    
    private void btnCarStatus_Click(object sender, RoutedEventArgs e)
    {
        CarStatusWindow carStatusWindow = new CarStatusWindow();
        carStatusWindow.Show();
    }
    
    private void btnNewCarSearch_Click(object sender, RoutedEventArgs e)
    {
        CarSearchWindow carSearchWindow = new CarSearchWindow();
        carSearchWindow.Show();
    }
    
    private void btnFindCar_Click(object sender, RoutedEventArgs e)
    {
        FindCarWindow findCarWindow = new FindCarWindow();
        findCarWindow.Show();
    }
    
    private void btnDeleteCar_Click(object sender, RoutedEventArgs e)
    {
        DeleteCarWindow deleteCarWindow = new DeleteCarWindow();
        deleteCarWindow.Show();
    }

    private void btnNewUsers_Click(object sender, RoutedEventArgs e)
    {
        NewUsers newUsersWindow = new NewUsers();
        newUsersWindow.Show();
    }
}