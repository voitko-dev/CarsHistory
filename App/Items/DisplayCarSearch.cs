
using CarsHistory.Extentions;

namespace CarsHistory.Items
{
    public class DisplayCarSearch : ObservableObject
    {
        public DisplayCarSearch(CarSearchItem carSearchItem)
        {
            Id = carSearchItem.Id;
            Brand = carSearchItem.Brand;
            Model = carSearchItem.Model;
            Group = carSearchItem.Group;
            Comment = carSearchItem.Comment;
            CarStatusClosed = carSearchItem.CarStatusClosed;
            LastPersonChange = carSearchItem.LastPersonChange;
            IsSelectedForDeletion = carSearchItem.IsSelectedForDeletion;
            DateUpdated = carSearchItem.DateUpdated;
            
            Auction_bca = carSearchItem.Auction_bca;
            Auction_autobid = carSearchItem.Auction_autobid;
            Auction_atc = carSearchItem.Auction_atc;
            Auction_ald = carSearchItem.Auction_ald;
            Auction_auto1 = carSearchItem.Auction_auto1;
            Auction_openlane = carSearchItem.Auction_openlane;
            Auction_autorola = carSearchItem.Auction_autorola;
            Auction_vw_finance = carSearchItem.Auction_vw_finance;
        }

        public CarSearchItem GetCarSearch(string currentUserName)
        {
            Func<FieldWithAuthor<DateTime?>, string, FieldWithAuthor<DateTime?>> finalizeField = (field, user) =>
            {
                if (field == null) 
                    return new FieldWithAuthor<DateTime?> { lastPersonChange = user };
                
                return new FieldWithAuthor<DateTime?>
                {
                    fieldValue = field.fieldValue?.ToUtcSafe(true),
                    lastPersonChange = string.IsNullOrEmpty(field.lastPersonChange) && field.fieldValue.HasValue
                        ? user
                        : field.lastPersonChange
                };
            };
            
            return new CarSearchItem
            {
                Id = Id,
                Brand = Brand,
                Model = Model,
                Group = Group,
                Comment = Comment,
                CarStatusClosed = CarStatusClosed,
                LastPersonChange = currentUserName,
                IsSelectedForDeletion = IsSelectedForDeletion,
                DateUpdated = DateTime.Now,
                
                Auction_bca = finalizeField(Auction_bca, currentUserName),
                Auction_autobid = finalizeField(Auction_autobid, currentUserName),
                Auction_atc = finalizeField(Auction_atc, currentUserName),
                Auction_ald = finalizeField(Auction_ald, currentUserName),
                Auction_auto1 = finalizeField(Auction_auto1, currentUserName),
                Auction_openlane = finalizeField(Auction_openlane, currentUserName),
                Auction_autorola = finalizeField(Auction_autorola, currentUserName),
                Auction_vw_finance = finalizeField(Auction_vw_finance, currentUserName)
            };
        }


        public string Id { get; set; }

        private string _brand;
        public string Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        private string _model;
        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }
        
        private string _group;
        public string Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        private bool _carStatusClosed;
        public bool CarStatusClosed
        {
            get => _carStatusClosed;
            set => SetProperty(ref _carStatusClosed, value);
        }

        private string _lastPersonChange;
        public string LastPersonChange
        {
            get => _lastPersonChange;
            set => SetProperty(ref _lastPersonChange, value);
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

        private FieldWithAuthor<DateTime?> _auction_bca;
        public FieldWithAuthor<DateTime?> Auction_bca
        {
            get => _auction_bca;
            set => SetProperty(ref _auction_bca, value);
        }

        private FieldWithAuthor<DateTime?> _auction_autobid;
        public FieldWithAuthor<DateTime?> Auction_autobid
        {
            get => _auction_autobid;
            set => SetProperty(ref _auction_autobid, value);
        }

        private FieldWithAuthor<DateTime?> _auction_atc;
        public FieldWithAuthor<DateTime?> Auction_atc
        {
            get => _auction_atc;
            set => SetProperty(ref _auction_atc, value);
        }

        private FieldWithAuthor<DateTime?> _auction_ald;
        public FieldWithAuthor<DateTime?> Auction_ald
        {
            get => _auction_ald;
            set => SetProperty(ref _auction_ald, value);
        }

        private FieldWithAuthor<DateTime?> _auction_auto1;
        public FieldWithAuthor<DateTime?> Auction_auto1
        {
            get => _auction_auto1;
            set => SetProperty(ref _auction_auto1, value);
        }

        private FieldWithAuthor<DateTime?> _auction_openlane;
        public FieldWithAuthor<DateTime?> Auction_openlane
        {
            get => _auction_openlane;
            set => SetProperty(ref _auction_openlane, value);
        }

        private FieldWithAuthor<DateTime?> _auction_autorola;
        public FieldWithAuthor<DateTime?> Auction_autorola
        {
            get => _auction_autorola;
            set => SetProperty(ref _auction_autorola, value);
        }

        private FieldWithAuthor<DateTime?> _auction_vw_finance;
        public FieldWithAuthor<DateTime?> Auction_vw_finance
        {
            get => _auction_vw_finance;
            set => SetProperty(ref _auction_vw_finance, value);
        }
    }
}