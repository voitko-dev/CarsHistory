using Firebase.Database;
using Firebase.Database.Query;
using CarsHistory.Items;

namespace CarsHistory.Services;

public class FirebaseService
{
    private static FirebaseService? _instance;
    private readonly FirebaseClient firebaseClient;
    private const string firebaseUrl = "https://carshistory.firebaseio.com/";

    // Конструктор приватний, щоб заборонити створення екземпляра ззовні
    private FirebaseService(string firebaseUrl)
    {
        firebaseClient = new FirebaseClient(firebaseUrl);
    }
    
    public static FirebaseService GetInstance()
    {
        return _instance ??= new FirebaseService(firebaseUrl);
    }

    // Додати новий автомобіль до бази даних Firebase
    /*public async Task<FirebaseObject<Car>> AddCarAsync(Car car)
    {
        var result = await firebaseClient
            .Child("cars")
            .PostAsync(new
            {
                brand = car.Brand,
                model = car.Model,
                price = car.Price,
                comment = car.Comment,
                mileage = car.Mileage,
                production_date = car.ProductionDate,
                auction_date = car.DateAdded,
                engine_power = car.EnginePower,
                fuel_type = car.FuelType,
                transmission = car.Transmission,
                photo_url = car.PhotoUrl
            });
        
        return result;

        Console.WriteLine($"Car added with ID: {result.Key}");
    }*/
    
    public async Task<FirebaseObject<Car>> AddCarAsync(Car car)
    {
        var result = await firebaseClient
            .Child("cars")
            .PostAsync(car);
        
        Console.WriteLine($"Car added with ID: {result.Key}");
        
        return result;
    }

    // Отримати всі автомобілі з Firebase
    public async Task<List<Car>> GetCarsAsync()
    {
        var cars = await firebaseClient
            .Child("cars")
            .OnceAsync<Car>();

        var carList = new List<Car>();
        foreach (var car in cars)
        {
            carList.Add(car.Object);
        }
        return carList;
    }
    
    // Отримати автомобіль за ID
    public async Task<Car> GetCarByIdAsync(string carId)
    {
        var carData = await firebaseClient
            .Child("cars")
            .Child(carId)
            .OnceSingleAsync<Car>();

        return carData;  // Повертаємо дані автомобіля або null, якщо не знайдено
    }

    // Видалити автомобіль за Id
    public async Task DeleteCarAsync(string id)
    {
        await firebaseClient
            .Child("cars")
            .Child(id)  // Використовуємо Id як унікальний ідентифікатор
            .DeleteAsync();
    }
}
