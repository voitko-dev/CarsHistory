using Google.Cloud.Firestore;

namespace CarsHistory.Items
{
    [FirestoreData]
    public class Car : ObservableObject
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
        
        private string carStatusId;
        [FirestoreProperty]
        public string CarStatusId
        {
            get => carStatusId;
            set => SetProperty(ref carStatusId, value);
        }

        private string model;
        [FirestoreProperty]
        public string Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        private double price;
        [FirestoreProperty]
        public double Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        private double customsCosts;
        [FirestoreProperty(Name = "СustomsСlearanceСosts")]
        public double СustomsСlearanceСosts
        {
            get => customsCosts;
            set => SetProperty(ref customsCosts, value);
        }

        private int mileage;
        [FirestoreProperty]
        public int Mileage
        {
            get => mileage;
            set => SetProperty(ref mileage, value);
        }

        private DateTime productionDate;
        [FirestoreProperty]
        public DateTime ProductionDate
        {
            get => productionDate;
            set => SetProperty(ref productionDate, value);
        }
        
        private DateTime auctionDate;
        [FirestoreProperty]
        public DateTime AuctionDate
        {
            get => auctionDate;
            set => SetProperty(ref auctionDate, value);
        }

        private DateTime dateAdded;
        [FirestoreProperty]
        public DateTime DateAdded
        {
            get => dateAdded;
            set => SetProperty(ref dateAdded, value);
        }

        private int enginePower;
        [FirestoreProperty]
        public int EnginePower
        {
            get => enginePower;
            set => SetProperty(ref enginePower, value);
        }
        
        private bool purchased;
        [FirestoreProperty]
        public bool Purchased
        {
            get => purchased;
            set => SetProperty(ref purchased, value);
        }

        private string fuelType;
        [FirestoreProperty]
        public string FuelType
        {
            get => fuelType;
            set => SetProperty(ref fuelType, value);
        }

        private string transmission;
        [FirestoreProperty]
        public string Transmission
        {
            get => transmission;
            set => SetProperty(ref transmission, value);
        }
        
        private string carFrom;
        [FirestoreProperty]
        public string CarFrom
        {
            get => carFrom;
            set => SetProperty(ref carFrom, value);
        }

        private string vin;
        [FirestoreProperty]
        public string VIN
        {
            get => vin;
            set => SetProperty(ref vin, value);
        }

        private string author;
        [FirestoreProperty]
        public string Author
        {
            get => author;
            set => SetProperty(ref author, value);
        }

        private string idAuthor;
        [FirestoreProperty]
        public string IdAuthor
        {
            get => idAuthor;
            set => SetProperty(ref idAuthor, value);
        }

        private string comment;
        [FirestoreProperty]
        public string Comment
        {
            get => comment;
            set => SetProperty(ref comment, value);
        }
    }
}
