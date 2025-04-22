using System.IO;
using System.IO.IsolatedStorage;

namespace CarsHistory.Services;

public class EmailSaverService
{
    private static EmailSaverService? _instance;
    
    public static EmailSaverService GetInstance()
    {
        return _instance ??= new EmailSaverService();
    }
    
    public List<string> LoadEmailList()
    {
        List<string> emails = new List<string>();
        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        {
            if (isoStore.FileExists("savedEmails.txt"))
            {
                using (IsolatedStorageFileStream stream =
                       new IsolatedStorageFileStream("savedEmails.txt", FileMode.Open, isoStore))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    emails = content.Split(';').Where(email => !string.IsNullOrEmpty(email)).ToList();
                }
            }
        }

        return emails;
    }
    
    public void SaveEmailList(List<string> emails)
    {
        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        {
            using (IsolatedStorageFileStream stream =
                   new IsolatedStorageFileStream("savedEmails.txt", FileMode.Create, isoStore))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(string.Join(";", emails));
            }
        }
    }
}