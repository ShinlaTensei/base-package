using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Base.Logging;
using UnityEngine;

namespace Base.Helper
{
    public static class Encryption
    {
        public enum HashType
        {
            /// <summary>
            /// MD5 Hash
            /// </summary>
            MD5,
            /// <summary>
            /// SHA256 Hash
            /// </summary>
            SHA256
        }
        private static readonly string _privateKey = "aYwxsNCz";

        private static Aes GetAesManaged(string privateKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, 1000);
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            key.Dispose();
            return aes;
        }

        private static byte[] Encrypt(byte[] plainByte, Aes aesManaged)
        {
            byte[] output;
            using (MemoryStream ms = new MemoryStream())
            {
                ICryptoTransform encryptor = aesManaged.CreateEncryptor();
                using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainByte, 0, plainByte.Length);
                    cryptoStream.Close();
                }

                output = ms.ToArray();
            }

            return output;
        }
        
        private static byte[] Decrypt(byte[] encryptedData, Aes aesManaged)
        {
            byte[] output;
            using (MemoryStream ms = new MemoryStream())
            {
                ICryptoTransform decryptor = aesManaged.CreateDecryptor();
                using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                    cryptoStream.Close();
                }

                output = ms.ToArray();
            }

            return output;
        }

        public static byte[] Encrypt(string content, string key)
        {
            using (var rijndaelManaged = GetAesManaged(key))
            {
                return Encrypt(Encoding.UTF8.GetBytes(content), rijndaelManaged);
            }
        }
        
        public static byte[] Encrypt(byte[] plainBytes, string key)
        {
            using (var rijndaelManaged = GetAesManaged(key))
            {
                return Encrypt(plainBytes, rijndaelManaged);
            }
        }
        
        public static byte[] Decrypt(byte[] data, string key)
        {
            try
            {
                using (var rijndaelManaged = GetAesManaged(key))
                {
                    return Decrypt(data, rijndaelManaged);
                }
            }
            catch (Exception ex)
            {
                PDebug.ErrorFormat("Decode Exception: " + ex.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Usage: Create a checksum string for small to moderately sized data
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="hashType">Type of the hash
        /// <list type="HashType">
        /// <item><description>MD5</description></item>
        /// <item><description>SHA256</description></item>
        /// </list>
        /// </param>
        /// <returns>Hash string</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ComputeHash(byte[] inputData, HashType hashType = HashType.MD5)
        {
            HashAlgorithm hashAlgorithm = null;
            switch (hashType)
            {
                case HashType.MD5:
                    hashAlgorithm = MD5.Create();
                    break;
                case HashType.SHA256:
                    hashAlgorithm = SHA256.Create();
                    break;
            }

            if (hashAlgorithm == null)
            {
                throw new NullReferenceException("Hash Algorithm is null");
            }
            byte[] md5Hash = hashAlgorithm.ComputeHash(inputData);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Hash.Length; ++i)
            {
                sb.Append(md5Hash[i].ToString("X2").ToLower(CultureInfo.CurrentCulture));
            }
            hashAlgorithm.Dispose();
            return sb.ToString();
        }

        public static string TransformHashSHA256(byte[] inputBytes, int chunkSize = 10)
        {
            using (SHA256 sha = SHA256.Create())
            {
                int offset = 0;
                while (offset < inputBytes.Length - chunkSize)
                {
                    sha.TransformBlock(inputBytes, offset, chunkSize, inputBytes, offset);
                    offset += chunkSize;
                }

                sha.TransformFinalBlock(inputBytes, 0, inputBytes.Length - offset);
                byte[] hash = sha.Hash;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2").ToLower(CultureInfo.CurrentCulture));
                }

                return sb.ToString();
            }
        }
        
        public static string TransformHashMD5(byte[] inputBytes, int chunkSize = 10)
        {
            using (MD5 md5 = MD5.Create())
            {
                int offset = 0;
                while (offset < inputBytes.Length - chunkSize)
                {
                    md5.TransformBlock(inputBytes, offset, chunkSize, inputBytes, offset);
                    offset += chunkSize;
                }

                md5.TransformFinalBlock(inputBytes, 0, inputBytes.Length - offset);
                byte[] hash = md5.Hash;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2").ToLower(CultureInfo.CurrentCulture));
                }

                return sb.ToString();
            }
        }
    }

}
