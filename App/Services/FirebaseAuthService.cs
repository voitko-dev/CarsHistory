using System.Drawing;
using System.Net.Http;
using System.Text;
using CarsHistory.Extentions;
using CarsHistory.Items;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json.Linq;

namespace CarsHistory.Services;

public partial class FirebaseService
{
    private static User? _currentUser;
    public static User? CurrentUser => _currentUser;

    public static async Task RegisterUserAsync(string email, string password, string role, string name)
    {
        UserRecord? userRecord = await GetInstance().Auth.CreateUserAsync(new UserRecordArgs()
        {
            Email = email,
            Password = password
        });

        string color = GenerateColorFromUserName(name);
        DocumentReference userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userRecord.Uid);
        await userRef.SetAsync(new { Role = role, Name = name, Email = email, Color = color, IsBlocked = false });

        Console.WriteLine($"Successfully created new user with name: {name} and role: {role}");
    }

    private static string? GetApiKey()
    {
        string decryptedJson =
            FileDecryption.DecryptFile(AppDomain.CurrentDomain.BaseDirectory + @"appsettings.json.enc");

        JObject config = JObject.Parse(decryptedJson);
        return config["FirebaseSettings"]?["ApiKey"]?.ToString();
    }

    private static string GenerateColorFromUserName(string userName)
    {
        int hash = userName.GetHashCode();
        byte red = (byte)(hash & 0xFF);
        byte green = (byte)((hash >> 8) & 0xFF);
        byte blue = (byte)((hash >> 16) & 0xFF);

        return Color.FromArgb(255, red, green, blue).ToHex();
    }

    public static async Task RegisterUserWithApproveAsync(string email, string password, string name)
    {
        UserRecord? userRecord = await GetInstance().Auth.CreateUserAsync(new UserRecordArgs()
        {
            Email = email,
            Password = password
        });

        string color = GenerateColorFromUserName(name);
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userRecord.Uid);
        await userRef.SetAsync(new { Role = "Pending", Email = email, Name = name, Color = color, IsBlocked = false });

        Console.WriteLine($"User {email} registered with pending status.");
    }

    public static async Task<List<KeyValuePair<string, string>>> GetPendingUsersAsync()
    {
        CollectionReference? usersRef = GetInstance().firestoreDb.Collection(UsersCollection);

        QuerySnapshot? snapshot = await usersRef.WhereEqualTo("Role", "Pending").GetSnapshotAsync();

        List<KeyValuePair<string, string>> pendingUsers = new List<KeyValuePair<string, string>>();
        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            Dictionary<string, object>? res = document.ToDictionary();
            pendingUsers.Add(new KeyValuePair<string, string>($"{res["Email"]}-{res["Name"]}", document.Id));
        }

        return pendingUsers;
    }
    
    public static async Task BlockUserAsync(string userUid, bool blocked)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userUid);
        await userRef.UpdateAsync("IsBlocked", blocked);

        Console.WriteLine($"User {userUid} has been {(blocked ? "blocked" : "unblocked")}.");
    }

    public static async Task ApproveUserAsync(string userUid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userUid);
        await userRef.UpdateAsync("Role", "user");

        Console.WriteLine($"User {userUid} has been approved.");
    }

    public static async Task<(string? userId, bool isBlocked)> SignInUserAsync(string email, string password)
    {
        HttpClient client = new HttpClient();
        JObject requestBody = new JObject
        {
            { "email", email },
            { "password", password },
            { "returnSecureToken", true }
        };
    
        using StringContent content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
        HttpResponseMessage response =
            await client.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={GetApiKey()}", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            JObject jsonResponse = JObject.Parse(responseContent);
            string? id = jsonResponse["localId"]?.ToString();
        
            if (id != null)
            {
                User? user = await LoadCurrentUserAsync(id);
            
                // Перевірка чи користувач заблокований
                if (user != null && user.IsBlocked)
                {
                    Console.WriteLine($"User {email} is blocked and cannot sign in.");
                    return (null, true); // Повертаємо прапорець isBlocked=true
                }
            }
        
            return (id, false); // Користувач не заблокований
        }

        Console.WriteLine($"Error: {responseContent}");
        return (null, false); // Помилка входу, але не через блокування
    }

    private static async Task<User?> LoadCurrentUserAsync(string userId)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userId);
        DocumentSnapshot? document = await userRef.GetSnapshotAsync();

        if (document.Exists)
        {
            Dictionary<string, object> userData = document.ToDictionary();
            string name = userData.GetValueOrDefault("Name", string.Empty)?.ToString() ?? string.Empty;
            string email = userData.GetValueOrDefault("Email", string.Empty)?.ToString() ?? string.Empty;
            string color = userData.GetValueOrDefault("Color", "#FFFFFF")?.ToString() ?? "#FFFFFF";
            string roleStr = userData.GetValueOrDefault("Role", string.Empty)?.ToString() ?? string.Empty;
            bool isBlocked = userData.GetValueOrDefault("IsBlocked", false) is true;
            UsersRole role = roleStr.GetRole();

            _currentUser = new User(userId, name, email, color, role, isBlocked);
            return _currentUser;
        }
        else
        {
            _currentUser = null;
            return null;
        }
    }

    public static async Task<UsersRole> GetUserRoleAsync(string uid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(uid);
        DocumentSnapshot? document = await userRef.GetSnapshotAsync();
        string role = (document.Exists ? document.ToDictionary()["Role"].ToString() : null) ?? string.Empty;
        return role.GetRole();
    }

    public static async Task<List<string>> GetAllUsersNameAsync()
    {
        CollectionReference userCollection = GetInstance().firestoreDb.Collection(UsersCollection);
        QuerySnapshot? snapshot = await userCollection.GetSnapshotAsync();
        List<string> usersNames = new List<string>();
        foreach (DocumentSnapshot? item in snapshot.Documents)
        {
            string name = (item.Exists ? item.ToDictionary()["Name"].ToString() : null) ?? string.Empty;
            if (name.IsNullOrEmpty())
                continue;

            usersNames.Add(name);
        }

        return usersNames;
    }

    public static async Task<Dictionary<string, string>> GetUsersColorsAsync()
    {
        CollectionReference userCollection = GetInstance().firestoreDb.Collection(UsersCollection);
        QuerySnapshot? snapshot = await userCollection.GetSnapshotAsync();
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (DocumentSnapshot? item in snapshot.Documents)
        {
            string name = (item.Exists ? item.ToDictionary()["Name"].ToString() : null) ?? string.Empty;
            string color = (item.Exists ? item.ToDictionary()?["Color"]?.ToString() : null) ?? string.Empty;
            if (name.IsNullOrEmpty() || color.IsNullOrEmpty())
                continue;

            data.Add(name, color);
        }

        return data;
    }

    public static async Task SetUserColorAsync(string userId, string color)
    {
        DocumentReference docRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userId);
        await docRef.UpdateAsync("Color", color);

        if (_currentUser?.Id == userId)
        {
            _currentUser.Color = color;
        }
    }

    public static async Task<string> GetUserColorAsync(string userId)
    {
        DocumentReference docRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.GetValue<string>("Color");
        }

        return "#FFFFFF";
    }

    public static async Task<User?> GetUserAsync(string uid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection(UsersCollection).Document(uid);
        DocumentSnapshot? document = await userRef.GetSnapshotAsync();

        if (document.Exists)
        {
            Dictionary<string, object> userData = document.ToDictionary();
            string name = userData.GetValueOrDefault("Name", string.Empty)?.ToString() ?? string.Empty;
            string email = userData.GetValueOrDefault("Email", string.Empty)?.ToString() ?? string.Empty;
            string color = userData.GetValueOrDefault("Color", "#FFFFFF")?.ToString() ?? "#FFFFFF";
            string roleStr = userData.GetValueOrDefault("Role", string.Empty)?.ToString() ?? string.Empty;
            bool isBlocked = userData.GetValueOrDefault("IsBlocked", false) is true;
            UsersRole role = roleStr.GetRole();

            return new User(uid, name, email, color, role, isBlocked);
        }

        return null;
    }

    public static async Task<(UsersRole, string)> GetUserRoleAndNameAsync(string uid)
    {
        User? user = await GetUserAsync(uid);
        return user != null ? (user.Role, user.Name) : (UsersRole.None, string.Empty);
    }
    
    public static async Task<List<User>> GetAllUsersAsync()
    {
        CollectionReference userCollection = GetInstance().firestoreDb.Collection("users");
        QuerySnapshot? snapshot = await userCollection.GetSnapshotAsync();
        List<User> users = new List<User>();
        
        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            if (document.Exists)
            {
                Dictionary<string, object> userData = document.ToDictionary();
                string name = userData.GetValueOrDefault("Name", string.Empty)?.ToString() ?? string.Empty;
                string email = userData.GetValueOrDefault("Email", string.Empty)?.ToString() ?? string.Empty;
                string color = userData.GetValueOrDefault("Color", "#FFFFFF")?.ToString() ?? "#FFFFFF";
                string roleStr = userData.GetValueOrDefault("Role", string.Empty)?.ToString() ?? string.Empty;
                bool isBlocked = userData.GetValueOrDefault("IsBlocked", false) is true;
                UsersRole role = roleStr.GetRole();
                
                users.Add(new User(document.Id, name, email, color, role, isBlocked));
            }
        }
        
        return users;
    }
}