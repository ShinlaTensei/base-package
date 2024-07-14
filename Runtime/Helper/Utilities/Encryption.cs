using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        private static RijndaelManaged GetRijndaelManaged(string privateKey)
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

        private static byte[] Encrypt(byte[] plainByte, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainByte, 0, plainByte.Length);
        }
        
        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static byte[] Encrypt(string content)
        {
            using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
            {
                return Encrypt(Encoding.UTF8.GetBytes(content), rijndaelManaged);
            }
        }
        
        public static byte[] Encrypt(byte[] plainBytes)
        {
            using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
            {
                return Encrypt(plainBytes, rijndaelManaged);
            }
        }
        
        public static byte[] Decrypt(byte[] data, Action<bool> result = null)
        {
            try
            {
                using (var rijndaelManaged = GetRijndaelManaged(_privateKey))
                {
                    result?.Invoke(obj: true);
                    return Decrypt(data, rijndaelManaged);
                }
            }
            catch (Exception ex)
            {
                PDebug.ErrorFormat("Decode Exception: " + ex.Message);
                result?.Invoke(obj: false);
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

        public static string TransformHash(byte[] inputBytes, int chunkSize = 10)
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
    }

}
