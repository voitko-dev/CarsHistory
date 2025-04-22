using CarsHistory.Services;
using CarsHistory.Extentions;

namespace CarsHistory.Items
{
    public class DisplayCarStatus : ObservableObject
    {
        public DisplayCarStatus(CarStatus carStatus, Car car)
        {
            Id = carStatus.Id;
            CarId = carStatus.CarId;
            _carStatusClosed = carStatus.CarStatusClosed;
            _brand = car.Brand;
            _model = car.Model;
            _vin = car.VIN;
            _auction = carStatus.Auction;
            _isCarPurchased = car.Purchased; // From Car object
            _dateUpdated = carStatus.DateUpdated;
            _isSelectedForDeletion = carStatus.IsSelectedForDeletion;
            _generalLastPersonChange = carStatus.LastPersonChange;

            // Initialize FieldWithAuthor properties, creating new if null in source
            string initialAuthor = string.IsNullOrEmpty(carStatus.LastPersonChange)
                ? FirebaseService.GetInstance().CurUserName
                : carStatus.LastPersonChange;

            _purchased = carStatus.Purchased ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _moneyTransferred = carStatus.MoneyTransferred ??
                                new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _moneyReceived = carStatus.MoneyReceived ?? new FieldWithAuthor<DateTime?>
                { lastPersonChange = initialAuthor };
            _moneyInBank = carStatus.MoneyInBank ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _carPaid = carStatus.CarPaid ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _documentsForSelection = carStatus.DocumentsForSelection ?? new FieldWithAuthor<DateTime?>
                { lastPersonChange = initialAuthor };
            _carInLublin = carStatus.CarInLublin ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _cmrClosed = carStatus.CmrClosed ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _docsReceived = carStatus.DocsReceived ?? new FieldWithAuthor<DateTime?>
                { lastPersonChange = initialAuthor };
            _carInLutsk = carStatus.CarInLutsk ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _carCleared = carStatus.CarCleared ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _carPrepared = carStatus.CarPrepared ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _carLoaded = carStatus.CarLoaded ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _carCertified = carStatus.CarCertified ?? new FieldWithAuthor<DateTime?>
                { lastPersonChange = initialAuthor };
            _carSold = carStatus.CarSold ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
            _vatRefunded = carStatus.VatRefunded ?? new FieldWithAuthor<DateTime?> { lastPersonChange = initialAuthor };
        }

        public CarStatus GetCarStatus(string currentUserName)
        {
            Func<FieldWithAuthor<DateTime?>, string, FieldWithAuthor<DateTime?>> finalizeField = (field, user) =>
            {
                if (field == null) return new FieldWithAuthor<DateTime?> { lastPersonChange = user };
                return new FieldWithAuthor<DateTime?>
                {
                    fieldValue = field.fieldValue?.ToUtcSafe(true),
                    lastPersonChange = string.IsNullOrEmpty(field.lastPersonChange) && field.fieldValue.HasValue
                        ? user
                        : field.lastPersonChange
                };
            };

            return new CarStatus()
            {
                Id = Id,
                CarId = CarId,
                Auction = _auction,
                Purchased = finalizeField(_purchased, currentUserName),
                MoneyTransferred = finalizeField(_moneyTransferred, currentUserName),
                MoneyReceived = finalizeField(_moneyReceived, currentUserName),
                MoneyInBank = finalizeField(_moneyInBank, currentUserName),
                CarPaid = finalizeField(_carPaid, currentUserName),
                DocumentsForSelection = finalizeField(_documentsForSelection, currentUserName),
                CarInLublin = finalizeField(_carInLublin, currentUserName),
                CmrClosed = finalizeField(_cmrClosed, currentUserName),
                DocsReceived = finalizeField(_docsReceived, currentUserName),
                CarInLutsk = finalizeField(_carInLutsk, currentUserName),
                CarCleared = finalizeField(_carCleared, currentUserName),
                CarPrepared = finalizeField(_carPrepared, currentUserName),
                CarLoaded = finalizeField(_carLoaded, currentUserName),
                CarCertified = finalizeField(_carCertified, currentUserName),
                CarSold = finalizeField(_carSold, currentUserName),
                VatRefunded = finalizeField(_vatRefunded, currentUserName),
                DateUpdated = _dateUpdated?.ToUtcSafe(true),
                CarStatusClosed = _carStatusClosed,
                IsSelectedForDeletion = _isSelectedForDeletion,
                LastPersonChange = currentUserName // The user saving the entire record now
            };
        }


        public string Id { get; set; }

        public string CarId { get; set; }

        private bool _carStatusClosed;

        public bool CarStatusClosed
        {
            get => _carStatusClosed;
            set => SetProperty(ref _carStatusClosed, value);
        }

        private bool _isCarPurchased;

        public bool CarPurchased
        {
            get => _isCarPurchased;
            set => SetProperty(ref _isCarPurchased, value);
        }

        private bool _isSelectedForDeletion;

        public bool IsSelectedForDeletion
        {
            get => _isSelectedForDeletion;
            set => SetProperty(ref _isSelectedForDeletion, value);
        }

        private DateTime? _dateUpdated;

        public DateTime? DateUpdated
        {
            get => _dateUpdated;
            set => SetProperty(ref _dateUpdated, value);
        }

        private string _brand;

        public string Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        private string _vin;

        public string VIN
        {
            get => _vin;
            set => SetProperty(ref _vin, value); // Or readonly
        }

        private string _model;

        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value); // Or readonly
        }

        private string _auction;

        public string Auction
        {
            get => _auction;
            set => SetProperty(ref _auction, value);
        }

        private FieldWithAuthor<DateTime?> _purchased;

        public FieldWithAuthor<DateTime?> Purchased
        {
            get => _purchased;
            set => SetProperty(ref _purchased, value);
        }

        private FieldWithAuthor<DateTime?> _moneyTransferred;

        public FieldWithAuthor<DateTime?> MoneyTransferred
        {
            get => _moneyTransferred;
            set => SetProperty(ref _moneyTransferred, value);
        }

        private FieldWithAuthor<DateTime?> _moneyReceived;

        public FieldWithAuthor<DateTime?> MoneyReceived
        {
            get => _moneyReceived;
            set => SetProperty(ref _moneyReceived, value);
        }

        private FieldWithAuthor<DateTime?> _moneyInBank;

        public FieldWithAuthor<DateTime?> MoneyInBank
        {
            get => _moneyInBank;
            set => SetProperty(ref _moneyInBank, value);
        }

        private FieldWithAuthor<DateTime?> _carPaid;

        public FieldWithAuthor<DateTime?> CarPaid
        {
            get => _carPaid;
            set => SetProperty(ref _carPaid, value);
        }

        private FieldWithAuthor<DateTime?> _documentsForSelection;

        public FieldWithAuthor<DateTime?> DocumentsForSelection
        {
            get => _documentsForSelection;
            set => SetProperty(ref _documentsForSelection, value);
        }

        private FieldWithAuthor<DateTime?> _carInLublin;

        public FieldWithAuthor<DateTime?> CarInLublin
        {
            get => _carInLublin;
            set => SetProperty(ref _carInLublin, value);
        }

        private FieldWithAuthor<DateTime?> _cmrClosed;

        public FieldWithAuthor<DateTime?> CmrClosed
        {
            get => _cmrClosed;
            set => SetProperty(ref _cmrClosed, value);
        }

        private FieldWithAuthor<DateTime?> _docsReceived;

        public FieldWithAuthor<DateTime?> DocsReceived
        {
            get => _docsReceived;
            set => SetProperty(ref _docsReceived, value);
        }

        private FieldWithAuthor<DateTime?> _carInLutsk;

        public FieldWithAuthor<DateTime?> CarInLutsk
        {
            get => _carInLutsk;
            set => SetProperty(ref _carInLutsk, value);
        }

        private FieldWithAuthor<DateTime?> _carCleared;

        public FieldWithAuthor<DateTime?> CarCleared
        {
            get => _carCleared;
            set => SetProperty(ref _carCleared, value);
        }

        private FieldWithAuthor<DateTime?> _carPrepared;

        public FieldWithAuthor<DateTime?> CarPrepared
        {
            get => _carPrepared;
            set => SetProperty(ref _carPrepared, value);
        }

        private FieldWithAuthor<DateTime?> _carLoaded;

        public FieldWithAuthor<DateTime?> CarLoaded
        {
            get => _carLoaded;
            set => SetProperty(ref _carLoaded, value);
        }

        private FieldWithAuthor<DateTime?> _carCertified;

        public FieldWithAuthor<DateTime?> CarCertified
        {
            get => _carCertified;
            set => SetProperty(ref _carCertified, value);
        }

        private FieldWithAuthor<DateTime?> _carSold;

        public FieldWithAuthor<DateTime?> CarSold
        {
            get => _carSold;
            set => SetProperty(ref _carSold, value);
        }

        private FieldWithAuthor<DateTime?> _vatRefunded;

        public FieldWithAuthor<DateTime?> VatRefunded
        {
            get => _vatRefunded;
            set => SetProperty(ref _vatRefunded, value);
        }

        private string _generalLastPersonChange;

        public string GeneralLastPersonChange
        {
            get => _generalLastPersonChange;
            set => SetProperty(ref _generalLastPersonChange, value);
        }
    }
}