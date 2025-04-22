using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

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

        // Для всіх нових потоків
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        base.OnStartup(e);
    }
}