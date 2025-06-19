using System.Windows;

namespace CarsHistory.Windows;

public partial class BlockedUserWindow : Window
{
    public BlockedUserWindow()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}