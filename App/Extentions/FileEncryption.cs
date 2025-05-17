using System.IO;
using System.Security.Cryptography;
using System.Text;
using CarsHistory.Items;

namespace CarsHistory.Extentions;

public class FileEncryption
{
    public static void EncryptFile(string inputFile, string outputFile)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(UsersRoleExtensions.GG);
            aesAlg.IV = new byte[16];  

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            {
                fsInput.CopyTo(cs);
            }
        }
    }
}
