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
    private static string curUserName = String.Empty;
    private static string? curUserID = String.Empty;
    private static UsersRole curUserRole = UsersRole.None;

    public static async Task RegisterUserAsync(string email, string password, string role, string name)
    {
        UserRecord? userRecord = await GetInstance().Auth.CreateUserAsync(new UserRecordArgs()
        {
            Email = email,
            Password = password
        });
        
        DocumentReference userRef = GetInstance().firestoreDb.Collection("users").Document(userRecord.Uid);
        await userRef.SetAsync(new { Role = role, Name = name, Color = GenerateColorFromUserName(name) });

        Console.WriteLine($"Successfully created new user with name: {name} and role: {role}");
    }
    
    private static string? GetApiKey()
    {
        string decryptedJson = FileDecryption.DecryptFile(AppDomain.CurrentDomain.BaseDirectory + @"appsettings.json.enc");
        
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
        
        DocumentReference? userRef = GetInstance().firestoreDb.Collection("users").Document(userRecord.Uid);
        await userRef.SetAsync(new { Role = "Pending", Email = email, Name = name, Color = GenerateColorFromUserName(name) });

        Console.WriteLine($"User {email} registered with pending status.");
    }
    
    public static async Task<List<KeyValuePair<string, string>>> GetPendingUsersAsync()
    {
        CollectionReference? usersRef = GetInstance().firestoreDb.Collection("users");
        
        QuerySnapshot? snapshot = await usersRef.WhereEqualTo("Role", "Pending").GetSnapshotAsync();

        List<KeyValuePair<string, string>> pendingUsers = new List<KeyValuePair<string, string>>();
        foreach (DocumentSnapshot? document in snapshot.Documents)
        {
            Dictionary<string, object>? res = document.ToDictionary();
            pendingUsers.Add(new KeyValuePair<string, string>($"{res["Email"]}-{res["Name"]}", document.Id));
        }

        return pendingUsers;
    }
    
    public static async Task ApproveUserAsync(string userUid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection("users").Document(userUid);
        await userRef.UpdateAsync("Role", "user");

        Console.WriteLine($"User {userUid} has been approved.");
    }

    public static async Task<string?> SignInUserAsync(string email, string password)
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
            curUserID = id;
            return id;
        }

        Console.WriteLine($"Error: {responseContent}");
        return null;
    }

    public static async Task<UsersRole> GetUserRoleAsync(string uid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection("users").Document(uid);
        DocumentSnapshot? document = await userRef.GetSnapshotAsync();
        string role = (document.Exists ? document.ToDictionary()["Role"].ToString() : null) ?? string.Empty;
        return role.GetRole();
    }
    
    public static async Task<List<string>> GetAllUsersNameAsync()
    {
        CollectionReference userCollection = GetInstance().firestoreDb.Collection("users");
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
        CollectionReference userCollection = GetInstance().firestoreDb.Collection("users");
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
        DocumentReference docRef = GetInstance().firestoreDb.Collection("users").Document(userId);
        await docRef.UpdateAsync("Color", color);
    }

    public static async Task<string> GetUserColorAsync(string userId)
    {
        DocumentReference docRef = GetInstance().firestoreDb.Collection("users").Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.GetValue<string>("Color");
        }
        
        return "#FFFFFF";
    }
    
    public static async Task<(UsersRole, string)> GetUserRoleAndNameAsync(string uid)
    {
        DocumentReference? userRef = GetInstance().firestoreDb.Collection("users").Document(uid);
        DocumentSnapshot? document = await userRef.GetSnapshotAsync();
        string userName = (document.Exists ? document.ToDictionary()["Name"].ToString() : null) ?? string.Empty;
        string role = (document.Exists ? document.ToDictionary()["Role"].ToString() : null) ?? string.Empty;
        curUserName = userName;
        curUserRole = role.GetRole();
        return (curUserRole, userName);
    }
}