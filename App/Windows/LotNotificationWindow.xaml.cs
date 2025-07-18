using System.Windows;
using System.Windows.Threading;
using CarsHistory.Items;

namespace CarsHistory.Windows
{
    public partial class LotNotificationWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly DateTime _targetEndDate;

        public LotNotificationWindow(OfferItem offer)
        {
            InitializeComponent();

            lblDescription.Text = $"Опис: {offer.Description}";

            string linkText = $"Посилання: {offer.Link}";
            Converter.UrlToHyperlinkConverter.ProcessTextForLinks(linkText, lblLink);

            _targetEndDate = offer.EndDate.Value.ToLocalTime();

            // Створюємо та налаштовуємо таймер
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += DispatcherTimer_Tick;
            _timer.Start();

            UpdateTimerText();

            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 10;
            Top = desktopWorkingArea.Bottom - Height - 10;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            TimeSpan timeLeft = _targetEndDate - DateTime.Now;

            if (timeLeft.TotalSeconds > 0)
            {
                lblTimeLeft.Text = $"Залишилось: {timeLeft.Minutes:D2} хв {timeLeft.Seconds:D2} сек";
            }
            else
            {
                lblTimeLeft.Text = "Час вийшов!";
                _timer.Stop();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _timer?.Stop();
        }
    }
}