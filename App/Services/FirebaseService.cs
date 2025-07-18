using System.IO;
using CarsHistory.Extentions;
using CarsHistory.Items;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace CarsHistory.Services;

public partial class FirebaseService
{
    private static FirebaseService? _instance;
    private readonly FirestoreDb firestoreDb;
    private static FirebaseApp firebaseApp;
    private static FirebaseAuth auth;


    private const string CarStatusesCollection = "carStatuses";
    private const string CarCollection = "cars";
    private const string CarsHistoryCollection = "cars-history";
    private const string CarsSearchCollection = "carssearch";
    private const string UsersCollection = "users";
    private const string OffersCollection = "offers";

    public FirebaseAuth Auth => auth;
    public string CurUserName => _currentUser?.Name ?? string.Empty;
    public string? CurUserId => _currentUser?.Id ?? string.Empty;
    public UsersRole CurUserRole => _currentUser?.Role ?? UsersRole.None;
    private string encryptedFilePath = AppDomain.CurrentDomain.BaseDirectory + @"cars-history.json.enc";

    private FirebaseService()
    {
        string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "empty.json");

        try
        {
            FileDecryption.DecryptFile(encryptedFilePath, credentialsPath);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialsPath),
            });

            firestoreDb = FirestoreDb.Create(CarsHistoryCollection);
            auth = FirebaseAuth.GetAuth(firebaseApp);
        }
        finally
        {
            if (File.Exists(credentialsPath))
            {
                File.WriteAllText(credentialsPath, string.Empty);
            }
        }
    }

    public static FirebaseService GetInstance()
    {
        return _instance ??= new FirebaseService();
    }

    public async Task AddCarAsync(Car car)
    {
        var carRef = firestoreDb.Collection(CarCollection).Document();
        await carRef.SetAsync(car);

        Console.WriteLine($"Car added with ID: {carRef.Id}");
    }

    public async Task<List<Car>> GetCarsAsync()
    {
        List<Car> cars = new List<Car>();
        QuerySnapshot?
            snapshot = await firestoreDb.Collection(CarCollection)
                .GetSnapshotAsync(); // Отримуємо всі документи з колекції "cars"

        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            Car? car = document.ConvertTo<Car>();
            car.Id = document.Id;
            cars.Add(car);
            car.ResetModified();
        }

        return cars;
    }

    public async Task<List<CarSearchItem>> GetCarsSearchAsync()
    {
        List<CarSearchItem> cars = new List<CarSearchItem>();
        QuerySnapshot? snapshot = await firestoreDb.Collection(CarsSearchCollection).GetSnapshotAsync();

        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            CarSearchItem? car = document.ConvertTo<CarSearchItem>();
            car.Id = document.Id;
            cars.Add(car);
            car.ResetModified();
        }

        return cars;
    }

    public async Task DeleteCarSearchAsync(string id)
    {
        DocumentReference? carRef = firestoreDb.Collection(CarsSearchCollection).Document(id);
        await carRef.DeleteAsync();
    }

    public async Task<List<Car>> GetCarsWithoutStatusAsync()
    {
        List<Car> carsList = await GetCarsAsync();
        List<Car> resultList = new List<Car>();

        foreach (Car car in carsList)
        {
            if (car.CarStatusId.IsNullOrEmpty())
                resultList.Add(car);
        }

        return resultList;
    }

    public async Task<List<Car>> GetCarsWithStatusAsync()
    {
        List<Car> carsList = await GetCarsAsync();
        List<Car> resultList = new List<Car>();

        foreach (Car car in carsList)
        {
            if (!car.CarStatusId.IsNullOrEmpty())
                resultList.Add(car);
        }

        return resultList;
    }

    public async Task ClearAuctionDatesAsync()
    {
        try
        {
            List<CarSearchItem> carSearchList = await GetCarsSearchAsync();

            foreach (CarSearchItem item in carSearchList)
            {
                // Очищаємо поля аукціонів
                item.Auction_bca = null;
                item.Auction_autobid = null;
                item.Auction_atc = null;
                item.Auction_ald = null;
                item.Auction_auto1 = null;
                item.Auction_openlane = null;
                item.Auction_autorola = null;
                item.Auction_vw_finance = null;
            }

            await SetNewCarSearchDataAsync(carSearchList);

            Console.WriteLine($"Успішно очищено дати аукціонів: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при очищенні дат аукціонів: {ex.Message}");
            throw;
        }
    }


    public async Task<List<CarStatus>> GetCarStatusesAsync()
    {
        List<CarStatus> carsStatusesList = new List<CarStatus>();
        QuerySnapshot? snapshot = await firestoreDb.Collection(CarStatusesCollection).GetSnapshotAsync();

        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            CarStatus? carStatus = document.ConvertTo<CarStatus>();
            carStatus.Id = document.Id;
            carsStatusesList.Add(carStatus);
            carStatus.ResetModified();
        }

        return carsStatusesList;
    }

    public async Task AddCarStatusAsync(CarStatus carStatus)
    {
        DocumentReference? carStatusRef = firestoreDb.Collection(CarStatusesCollection).Document();

        carStatus.DateUpdated = DateTime.UtcNow;
        carStatus.LastPersonChange = CurUserName;

        await carStatusRef.SetAsync(carStatus);
    }

    public async Task SetNewCarStatusDataAsync(List<CarStatus>? carsStatusList)
    {
        if (carsStatusList != null)
        {
            foreach (CarStatus carStatus in carsStatusList.Where(t => t is { IsModified: true }))
            {
                carStatus.DateUpdated = DateTime.UtcNow;
                carStatus.LastPersonChange = CurUserName;
                DocumentReference docRef = firestoreDb.Collection(CarStatusesCollection).Document(carStatus.Id);
                await docRef.SetAsync(carStatus);
            }
        }
    }

    public async Task SetNewCarSearchDataAsync(List<CarSearchItem>? carSearchesList)
    {
        if (carSearchesList != null)
        {
            foreach (CarSearchItem carStatus in carSearchesList.Where(t => t is { IsModified: true }))
            {
                carStatus.DateUpdated = DateTime.UtcNow;
                carStatus.LastPersonChange = CurUserName;
                DocumentReference docRef = firestoreDb.Collection(CarsSearchCollection).Document(carStatus.Id);
                await docRef.SetAsync(carStatus);
            }
        }
    }

    public async Task AddCarSearchAsync(CarSearchItem carSearchItem)
    {
        try
        {
            CollectionReference carSearchesRef = firestoreDb.Collection(CarsSearchCollection);
            DocumentReference addedDocRef = await carSearchesRef.AddAsync(carSearchItem);

            await addedDocRef.SetAsync(carSearchItem);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding car search: {ex.Message}");
            throw;
        }
    }


    public async Task SetCarStatusFieldInCarDataAsync(string carId, string statusId = "")
    {
        DocumentReference? docRef = firestoreDb.Collection(CarCollection).Document(carId);

        if (docRef == null)
            return;

        DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();

        Car? car = documentSnapshot.ConvertTo<Car>();

        car.CarStatusId = statusId;

        car.ResetModified();
        await docRef.SetAsync(car);
    }

    public async Task SetNewCarDataAsync(List<Car>? cars)
    {
        if (cars != null)
        {
            foreach (Car car in cars.Where(t => t is { IsModified: true }))
            {
                if (CurUserRole == UsersRole.Admin || CurUserRole == UsersRole.SuperAdmin || car.IdAuthor == CurUserId)
                {
                    DocumentReference docRef = firestoreDb.Collection(CarCollection).Document(car.Id);
                    await docRef.SetAsync(car);
                }
            }
        }
    }

    public async Task DeleteCarStatusAsync(string id)
    {
        DocumentReference? carRef = firestoreDb.Collection(CarStatusesCollection).Document(id);
        List<Car> carsList = await GetCarsAsync();

        string carId = carsList.FirstOrDefault(t => t.CarStatusId == id)?.Id ?? "";

        if (!carId.IsNullOrEmpty())
            await SetCarStatusFieldInCarDataAsync(carId);

        await carRef.DeleteAsync();
    }

    public async Task DeleteCarAsync(string id)
    {
        DocumentReference? carRef = firestoreDb.Collection(CarCollection).Document(id);
        await carRef.DeleteAsync();
    }

    public async Task<List<OfferItem>> GetOffersByCarSearchIdsAsync(List<string> carSearchIds)
    {
        var offers = new List<OfferItem>();
        if (carSearchIds == null || !carSearchIds.Any())
        {
            return offers;
        }

        Query query = firestoreDb.Collection(OffersCollection).WhereIn("CarSearchItemId", carSearchIds);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            OfferItem offer = doc.ConvertTo<OfferItem>();
            offer.Id = doc.Id;
            offers.Add(offer);
        }

        return offers;
    }

    public async Task<List<string>> GetCarSearchGroupsAsync()
    {
        var carSearches = await GetCarsSearchAsync();
        return carSearches
            .Select(cs => cs.Group)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    public async Task<List<CarSearchItem>> GetCarsSearchByGroupAsync(string group)
    {
        List<CarSearchItem> cars = new List<CarSearchItem>();
        Query query = firestoreDb.Collection(CarsSearchCollection).WhereEqualTo("Group", group);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            CarSearchItem car = document.ConvertTo<CarSearchItem>();
            car.Id = document.Id;
            cars.Add(car);
        }

        return cars;
    }

    public async Task<List<OfferItem>> GetAllOffersAsync()
    {
        var offers = new List<OfferItem>();
        QuerySnapshot snapshot = await firestoreDb.Collection(OffersCollection).GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            OfferItem offer = doc.ConvertTo<OfferItem>();
            offer.Id = doc.Id;
            offers.Add(offer);
        }
        return offers;
    }

    public async Task AddOfferAsync(OfferItem offer)
    {
        await firestoreDb.Collection(OffersCollection).AddAsync(offer);
    }

    public async Task DeleteOffersAsync(List<string> offerIds)
    {
        if (offerIds == null || !offerIds.Any())
            return;

        WriteBatch batch = firestoreDb.StartBatch();
        foreach (var offerId in offerIds)
        {
            DocumentReference docRef = firestoreDb.Collection(OffersCollection).Document(offerId);
            batch.Delete(docRef);
        }
        await batch.CommitAsync();
    }

    public async Task<List<OfferItem>> GetAllPlayingOffersAsync()
    {
        var offers = new List<OfferItem>();
        QuerySnapshot snapshot = await firestoreDb.Collection(OffersCollection).GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            OfferItem offer = doc.ConvertTo<OfferItem>();

            if (offer.Status != OfferStatus.Playing)
                continue;

            offer.Id = doc.Id;
            offers.Add(offer);
        }
        return offers;
    }

    public async Task UpdateOffersAsync(List<OfferItem> offersToUpdate)
    {
        WriteBatch batch = firestoreDb.StartBatch();
        foreach (var offer in offersToUpdate)
        {
            DocumentReference docRef = firestoreDb.Collection(OffersCollection).Document(offer.Id);
            batch.Set(docRef, offer, SetOptions.MergeAll);
        }

        await batch.CommitAsync();
    }
}