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
    public FirebaseAuth Auth => auth;
    public string CurUserName => curUserName;
    public string? CurUserId => curUserID;
    public UsersRole CurUserRole => curUserRole;
    private string encryptedFilePath = AppDomain.CurrentDomain.BaseDirectory + @"cars-history.json.enc";

    private FirebaseService()
    {
        string decryptedJson = FileDecryption.DecryptFile(encryptedFilePath);

        firestoreDb = FirestoreDb.Create("cars-history");
        firebaseApp = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(decryptedJson),
        });
        
        auth = FirebaseAuth.GetAuth(firebaseApp);
    }
    
    public static FirebaseService GetInstance()
    {
        return _instance ??= new FirebaseService();
    }
    
    public async Task AddCarAsync(Car car)
    {
        var carRef = firestoreDb.Collection("cars").Document();  // Створюємо новий документ в колекції "cars"
        await carRef.SetAsync(car);
        
        Console.WriteLine($"Car added with ID: {carRef.Id}");
    }
    
    public async Task<List<Car>> GetCarsAsync()
    {
        List<Car> cars = new List<Car>();
        QuerySnapshot? snapshot = await firestoreDb.Collection("cars").GetSnapshotAsync();  // Отримуємо всі документи з колекції "cars"
            
        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            Car? car = document.ConvertTo<Car>();
            car.Id = document.Id; 
            cars.Add(car);
            car.ResetModified();
        }
        
        return cars;
    }
    
    public async Task<List<Car>> GetCarsWithoutStatusAsync()
    {
        List<Car> carsList = await GetCarsAsync();
        List<Car> resultList = new List<Car>();

        foreach (Car car in carsList)
        {
            if (car.CarStatusId.isNullOrEmpty())
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
            if (!car.CarStatusId.isNullOrEmpty())
                resultList.Add(car);
        }

        return resultList;
    }
    
    public async Task<List<CarStatus>> GetCarStatusesAsync()
    {
        List<CarStatus> carsStatusesList = new List<CarStatus>();
        QuerySnapshot? snapshot = await firestoreDb.Collection("carStatuses").GetSnapshotAsync();
        
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
        DocumentReference? carStatusRef = firestoreDb.Collection("carStatuses").Document();
        
        carStatus.DateUpdated = DateTime.UtcNow;
        carStatus.LastPersonChange = curUserName;
        
        await carStatusRef.SetAsync(carStatus);
    }
    
    public async Task SetNewCarStatusDataAsync(List<CarStatus>? carsStatusList)
    {
        if (carsStatusList != null)
        {
            foreach (CarStatus carStatus in carsStatusList.Where(t => t is { IsModified: true }))
            {
                carStatus.DateUpdated = DateTime.UtcNow;
                carStatus.LastPersonChange = curUserName;
                DocumentReference docRef = firestoreDb.Collection("carStatuses").Document(carStatus.Id);
                await docRef.SetAsync(carStatus);
            }
        }
    }

    public async Task SetCarStatusFieldInCarDataAsync(string carId, string statusId = "")
    {
        DocumentReference? docRef = firestoreDb.Collection("cars").Document(carId);
        
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
                if (curUserRole == UsersRole.Admin || car.IdAuthor == curUserID)
                {
                    DocumentReference docRef = firestoreDb.Collection("cars").Document(car.Id);
                    await docRef.SetAsync(car);
                }
            }
        }
    }
    
    public async Task DeleteCarStatusAsync(string id)
    {
        DocumentReference? carRef = firestoreDb.Collection("carStatuses").Document(id);
        List<Car> carsList = await GetCarsAsync();

        string carId = carsList.FirstOrDefault(t => t.CarStatusId == id)?.Id ?? "";
        
        if (!carId.isNullOrEmpty())
            await SetCarStatusFieldInCarDataAsync(carId);
        
        await carRef.DeleteAsync();
    }
    
    public async Task DeleteCarAsync(string id)
    {
        DocumentReference? carRef = firestoreDb.Collection("cars").Document(id);
        await carRef.DeleteAsync();
    }
}
