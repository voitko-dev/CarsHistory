using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using CarsHistory.Items;
using CarsHistory.Windows;

namespace CarsHistory.Services
{
    public class NotificationService
    {
        private static NotificationService _instance;
        private readonly FirebaseService _firebaseService;
        private Timer _timer;
        // Список ID оферів, про які ми вже сповістили, щоб не повторюватись
        private readonly HashSet<string> _notifiedOfferIds = new HashSet<string>();

        private NotificationService()
        {
            _firebaseService = FirebaseService.GetInstance();
        }

        public static NotificationService GetInstance()
        {
            return _instance ??= new NotificationService();
        }

        public void Start()
        {
            // Таймер буде спрацьовувати кожну хвилину
            _timer = new Timer(CheckOffers, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private async void CheckOffers(object state)
        {
            try
            {
                var playingOffers = await _firebaseService.GetAllPlayingOffersAsync();
                var now = DateTime.UtcNow;

                foreach (var offer in playingOffers)
                {
                    if (offer.EndDate.HasValue && !_notifiedOfferIds.Contains(offer.Id))
                    {
                        var endDateUtc = DateTime.SpecifyKind(offer.EndDate.Value, DateTimeKind.Utc);

                        var timeLeft = endDateUtc - now;

                        if (timeLeft > TimeSpan.Zero && timeLeft <= TimeSpan.FromMinutes(15))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                LotNotificationWindow notification = new LotNotificationWindow(offer);
                                notification.Show();
                            });

                            _notifiedOfferIds.Add(offer.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка в сервісі сповіщень: {ex.Message}");
            }
        }
    }
}