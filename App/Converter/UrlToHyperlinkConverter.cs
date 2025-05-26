using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace CarsHistory.Converter;

public static class UrlToHyperlinkConverter
{
    private static readonly Regex UrlRegex = new Regex(@"(http|https|ftp|www\.):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);

    public static void ProcessTextForLinks(string text, TextBlock textBlock)
    {
        if (string.IsNullOrEmpty(text))
            return;

        textBlock.Inlines.Clear();

        int lastIndex = 0;
        foreach (Match match in UrlRegex.Matches(text))
        {
            // Додати звичайний текст перед посиланням
            if (match.Index > lastIndex)
            {
                string plainText = text.Substring(lastIndex, match.Index - lastIndex);
                textBlock.Inlines.Add(new Run(plainText));
            }

            // Додати гіперпосилання
            string url = match.Value;
            Hyperlink hyperlink = new Hyperlink(new Run(url))
            {
                NavigateUri = new Uri(EnsureHttpPrefix(url)),
                Foreground = Brushes.DeepSkyBlue
            };

            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock.Inlines.Add(hyperlink);

            lastIndex = match.Index + match.Length;
        }

        // Додати залишок тексту після останнього посилання
        if (lastIndex < text.Length)
        {
            string remainingText = text.Substring(lastIndex);
            textBlock.Inlines.Add(new Run(remainingText));
        }
    }

    private static string EnsureHttpPrefix(string url)
    {
        if (url.StartsWith("www."))
            return "http://" + url;
        return url;
    }

    private static void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка при відкритті посилання: {ex.Message}", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}