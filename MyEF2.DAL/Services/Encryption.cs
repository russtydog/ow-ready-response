using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class Encryption
    {
        //two functions here, one will encrypt a string, the other will decrypt a string
        public static string Encrypt(string input)
        {
            //a strong cipher to encrypt the string using a key
            byte[] data = UTF8Encoding.UTF8.GetBytes(input);
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("$Monday1"));
                using (System.Security.Cryptography.TripleDESCryptoServiceProvider trip = new System.Security.Cryptography.TripleDESCryptoServiceProvider()
                {
                    Key = key,
                    Mode = System.Security.Cryptography.CipherMode.ECB,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7
                })
                {
                    System.Security.Cryptography.ICryptoTransform transform = trip.CreateEncryptor();
                    byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(result, 0, result.Length);
                }
            }
        }
        public static string Decrypt(string input)
        {
            //a strong cipher to decrypt the string using a key
            byte[] data = Convert.FromBase64String(input);
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("$Monday1"));
                using (System.Security.Cryptography.TripleDESCryptoServiceProvider trip = new System.Security.Cryptography.TripleDESCryptoServiceProvider()
                {
                    Key = key,
                    Mode = System.Security.Cryptography.CipherMode.ECB,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7
                })
                {
                    System.Security.Cryptography.ICryptoTransform transform = trip.CreateDecryptor();
                    byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(result);
                }
            }
        }
    }
}
