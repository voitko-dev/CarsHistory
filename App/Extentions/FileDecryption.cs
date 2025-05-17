using System.IO;
using System.Security.Cryptography;
using System.Text;
using CarsHistory.Items;

namespace CarsHistory.Extentions;

public class FileDecryption
{
    public static string DecryptFile(string inputFile)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(UsersRoleExtensions.GG);
            aesAlg.IV = new byte[16];

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            using (CryptoStream cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
    
    public static void DecryptFile(string inputFile, string outputFile)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(UsersRoleExtensions.GG);
            aesAlg.IV = new byte[16];

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            using (CryptoStream cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            {
                cs.CopyTo(fsOutput);
            }
        }
    }
}