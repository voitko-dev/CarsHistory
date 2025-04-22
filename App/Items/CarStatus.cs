using System.ComponentModel;
using System.Runtime.CompilerServices;
using CarsHistory.Converter;
using Google.Cloud.Firestore;

namespace CarsHistory.Items
{
    [FirestoreData]
    public class CarStatus : ObservableObject
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string CarId { get; set; }

        private bool closed;
        [FirestoreProperty]
        public bool CarStatusClosed
        {
            get => closed;
            set => SetProperty(ref closed, value);
        }
        
        private string lastPersonChange; 
        [FirestoreProperty]
        public string LastPersonChange
        {
            get => lastPersonChange;
            set => SetProperty(ref lastPersonChange, value);
        }
        
        private bool isSelectedForDeletion;
        [FirestoreProperty]
        public bool IsSelectedForDeletion
        {
            get => isSelectedForDeletion;
            set => SetProperty(ref isSelectedForDeletion, value);
        }
        
        private DateTime? dateUpdated;
        [FirestoreProperty]
        public DateTime? DateUpdated
        {
            get => dateUpdated;
            set => SetProperty(ref dateUpdated, value);
        }
        
        private string auction;
        [FirestoreProperty]
        public string Auction
        {
            get => auction;
            set => SetProperty(ref auction, value);
        }

        private FieldWithAuthor<DateTime?> purchased;
        [FirestoreProperty("Purchased", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Purchased
        {
            get => purchased;
            set => SetProperty(ref purchased, value);
        }

        private FieldWithAuthor<DateTime?> moneyTransferred;
        [FirestoreProperty("MoneyTransferred", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> MoneyTransferred
        {
            get => moneyTransferred;
            set => SetProperty(ref moneyTransferred, value);
        }

        private FieldWithAuthor<DateTime?> moneyReceived;
        [FirestoreProperty("MoneyReceived", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> MoneyReceived
        {
            get => moneyReceived;
            set => SetProperty(ref moneyReceived, value);
        }

        private FieldWithAuthor<DateTime?> moneyInBank;
        [FirestoreProperty("MoneyInBank", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> MoneyInBank
        {
            get => moneyInBank;
            set => SetProperty(ref moneyInBank, value);
        }

        private FieldWithAuthor<DateTime?> carPaid;
        [FirestoreProperty("CarPaid", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarPaid
        {
            get => carPaid;
            set => SetProperty(ref carPaid, value);
        }
        
        private FieldWithAuthor<DateTime?> documentsForSelection;
        [FirestoreProperty("DocumentsForSelection", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> DocumentsForSelection
        {
            get => documentsForSelection;
            set => SetProperty(ref documentsForSelection, value);
        }

        private FieldWithAuthor<DateTime?> carLoaded;
        [FirestoreProperty("CarLoaded", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarLoaded
        {
            get => carLoaded;
            set => SetProperty(ref carLoaded, value);
        }
        
        private FieldWithAuthor<DateTime?> carInLublin;
        [FirestoreProperty("CarInLublin", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarInLublin
        {
            get => carInLublin;
            set => SetProperty(ref carInLublin, value);
        }
        
        private FieldWithAuthor<DateTime?> cmrClosed;
        [FirestoreProperty("CmrClosed", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CmrClosed
        {
            get => cmrClosed;
            set => SetProperty(ref cmrClosed, value);
        }
        
        private FieldWithAuthor<DateTime?> docsReceived;
        [FirestoreProperty("DocsReceived", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> DocsReceived
        {
            get => docsReceived;
            set => SetProperty(ref docsReceived, value);
        }

        private FieldWithAuthor<DateTime?> carInLutsk;
        [FirestoreProperty("CarInLutsk", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarInLutsk
        {
            get => carInLutsk;
            set => SetProperty(ref carInLutsk, value);
        }
        
        private FieldWithAuthor<DateTime?> carCleared;
        [FirestoreProperty("CarCleared", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarCleared
        {
            get => carCleared;
            set => SetProperty(ref carCleared, value);
        }

        private FieldWithAuthor<DateTime?> carCertified;
        [FirestoreProperty("CarCertified", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarCertified
        {
            get => carCertified;
            set => SetProperty(ref carCertified, value);
        }
        
        private FieldWithAuthor<DateTime?> carPrepared;
        [FirestoreProperty("CarPrepared", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarPrepared
        {
            get => carPrepared;
            set => SetProperty(ref carPrepared, value);
        }

        private FieldWithAuthor<DateTime?> carSold;
        [FirestoreProperty("CarSold", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> CarSold
        {
            get => carSold;
            set => SetProperty(ref carSold, value);
        }

        private FieldWithAuthor<DateTime?> vatRefunded;
        [FirestoreProperty("VatRefunded", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> VatRefunded
        {
            get => vatRefunded;
            set => SetProperty(ref vatRefunded, value);
        }
    }

    [FirestoreData]
    public class FieldWithAuthor<T> : INotifyPropertyChanged
    {
    public T fieldValue;

    public string lastPersonChange;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    }
}
