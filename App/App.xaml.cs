using System.Windows;
using System.Globalization;
using CarsHistory.Services;


namespace CarsHistory;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        CultureInfo culture = new CultureInfo("uk-UA");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        NotificationService.GetInstance().Start();

        // Для всіх нових потоків
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        base.OnStartup(e);
    }
}