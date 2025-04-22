namespace CarsHistory.Services;

public class UsersService
{
    private static UsersService? _instance;
    
    private  Dictionary<string, string>? colorData = null;
    
    public Dictionary<string, string> ColorData => colorData;
    
    public static UsersService GetInstance()
    {
        return _instance ??= new UsersService();
    }

    public async void TakeUserData()
    {
        colorData ??= await FirebaseService.GetUsersColorsAsync();
    }
}