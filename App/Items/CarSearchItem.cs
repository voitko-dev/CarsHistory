using System.ComponentModel;
using System.Runtime.CompilerServices;
using CarsHistory.Converter;
using Google.Cloud.Firestore;

namespace CarsHistory.Items
{
    [FirestoreData]
    public class CarSearchItem : ObservableObject
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        
        
        private string brand;
        [FirestoreProperty]
        public string Brand
        {
            get => brand;
            set => SetProperty(ref brand, value);
        }

        private string model;
        [FirestoreProperty]
        public string Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }
        
        private string group;
        [FirestoreProperty]
        public string Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }
        
        private string comment;
        [FirestoreProperty]
        public string Comment
        {
            get => comment;
            set => SetProperty(ref comment, value);
        }

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
        
        private FieldWithAuthor<DateTime?> auction_bca;
        [FirestoreProperty("Auction_bca", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_bca
        {
            get => auction_bca;
            set => SetProperty(ref auction_bca, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_autobid;
        [FirestoreProperty("Auction_autobid", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_autobid
        {
            get => auction_autobid;
            set => SetProperty(ref auction_autobid, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_atc;
        [FirestoreProperty("Auction_atc", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_atc
        {
            get => auction_atc;
            set => SetProperty(ref auction_atc, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_ald;
        [FirestoreProperty("Auction_ald", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_ald
        {
            get => auction_ald;
            set => SetProperty(ref auction_ald, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_auto1;
        [FirestoreProperty("Auction_auto1", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_auto1
        {
            get => auction_auto1;
            set => SetProperty(ref auction_auto1, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_openlane;
        [FirestoreProperty("Auction_openlane", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_openlane
        {
            get => auction_openlane;
            set => SetProperty(ref auction_openlane, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_autorola;
        [FirestoreProperty("Auction_autorola", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_autorola
        {
            get => auction_autorola;
            set => SetProperty(ref auction_autorola, value);
        }
        
        private FieldWithAuthor<DateTime?> auction_vw_finance;
        [FirestoreProperty("Auction_vw_finance", ConverterType = typeof(FieldWithAuthorConverter<DateTime?>))]
        public FieldWithAuthor<DateTime?> Auction_vw_finance
        {
            get => auction_vw_finance;
            set => SetProperty(ref auction_vw_finance, value);
        }
    }
}
