using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Base.Helper
{
    public static class Encryption
    {
        private static readonly string _privateKey = "aYwxsNCz";

        public static RijndaelManaged GetRijndaelManaged(string privateKey)
        {
            byte[] array = new byte[16];
            byte[] key = Encoding.UTF8.GetBytes(privateKey);
            Array.Copy(key, array, Math.Min(array.Length, key.Length));
            return new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = array,
                IV = array
            };
        }

        public static byte[] Encrypt(byte[] plainByte, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainByte, 0, plainByte.Length);
        }
        
        public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static string Encrypt(string content)
        {
            using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
            {
                return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(content), rijndaelManaged));
            }
        }
        
        public static string Encrypt(byte[] plainBytes)
        {
            using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
            {
                return Convert.ToBase64String(Encrypt(plainBytes, rijndaelManaged));
            }
        }
        
        public static string Decrypt(string encryptedText, Action<bool> result = null)
        {
            try
            {
                using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
                {
                    byte[] encryptedData = Convert.FromBase64String(encryptedText);
                    result?.Invoke(obj: true);
                    return Encoding.UTF8.GetString(Decrypt(encryptedData, rijndaelManaged));
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("Decode Exception: " + ex.Message);
                result?.Invoke(obj: false);
                return string.Empty;
            }
        }

        public static string ToMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputHash = Encoding.UTF8.GetBytes(input);
                byte[] md5Hash = md5.ComputeHash(inputHash);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < md5Hash.Length; ++i)
                {
                    sb.Append(md5Hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }

}
