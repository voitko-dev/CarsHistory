using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Globalization;


namespace CarsHistory;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; }
    
    static App()
    {
        // Налаштування конфігурації
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
    }
    
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