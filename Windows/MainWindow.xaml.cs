using System.Windows;

namespace CarsHistory.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Обробка натискання на "Add New Car"
    private void MenuItem_AddCar_Click(object sender, RoutedEventArgs e)
    {
        // Відкриваємо вікно для додавання нового автомобіля
        var addCarWindow = new AddCarWindow(); // Створимо окреме вікно для додавання авто
        addCarWindow.Show();
    }

    // Обробка натискання на "Find Car"
    private void MenuItem_FindCar_Click(object sender, RoutedEventArgs e)
    {
        // Відкриваємо вікно для пошуку автомобіля
        var findCarWindow = new FindCarWindow(); // Створимо вікно для пошуку
        findCarWindow.Show();
    }

    // Обробка натискання на "Delete Car"
    private void MenuItem_DeleteCar_Click(object sender, RoutedEventArgs e)
    {
        // Відкриваємо вікно для видалення автомобіля
        var deleteCarWindow = new DeleteCarWindow(); // Створимо вікно для видалення
        deleteCarWindow.Show();
    }
}