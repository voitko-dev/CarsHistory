using CarsHistory.Items;
using Google.Cloud.Firestore;
using System;

namespace CarsHistory.Items
{
    // Перерахування для статусу оферу
    public enum OfferStatus
    {
        NotSelected, // Не обрано
        Playing,     // Так
        NotPlaying   // Ні
    }

    [FirestoreData]
    public class OfferItem : ObservableObject
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        // ID запису CarSearch, до якого прив'язаний офер
        private string _carSearchItemId;
        [FirestoreProperty]
        public string CarSearchItemId
        {
            get => _carSearchItemId;
            set => SetProperty(ref _carSearchItemId, value);
        }

        private string _groupName;
        [FirestoreProperty]
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        private string _link;
        [FirestoreProperty]
        public string Link
        {
            get => _link;
            set => SetProperty(ref _link, value);
        }

        private string _description;
        [FirestoreProperty]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private DateTime? _endDate;
        [FirestoreProperty]
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private double _maxPrice;
        [FirestoreProperty]
        public double MaxPrice
        {
            get => _maxPrice;
            set => SetProperty(ref _maxPrice, value);
        }

        private OfferStatus _status;
        [FirestoreProperty]
        public OfferStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private bool _isSelectedForDeletion;
        public bool IsSelectedForDeletion
        {
            get => _isSelectedForDeletion;
            set => SetProperty(ref _isSelectedForDeletion, value);
        }

        private string _lastChangeAuthor;
        [FirestoreProperty]
        public string LastChangeAuthor
        {
            get => _lastChangeAuthor;
            set => SetProperty(ref _lastChangeAuthor, value);
        }
    }
}