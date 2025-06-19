using Google.Cloud.Firestore;

namespace CarsHistory.Items;

public class User : ObservableObject
{
    private string _id;
    private string _name;
    private string _email;
    private string _color;
    private UsersRole _role;
    private bool _isBlocked;

    [FirestoreDocumentId]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    
    [FirestoreProperty]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    [FirestoreProperty]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    [FirestoreProperty]
    public string Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    [FirestoreProperty("Role")]
    public UsersRole Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }
    
    [FirestoreProperty]
    public bool IsBlocked
    {
        get => _isBlocked;
        set => SetProperty(ref _isBlocked, value);
    }

    public User()
    {
        _id = string.Empty;
        _name = string.Empty;
        _email = string.Empty;
        _color = "#FFFFFF";
        _role = UsersRole.None;
        _isBlocked = false;
    }

    public User(string id, string name, string email, string color, UsersRole role, bool isBlocked = false)
    {
        _id = id;
        _name = name;
        _email = email;
        _color = color;
        _role = role;
        _isBlocked = isBlocked;
    }
}