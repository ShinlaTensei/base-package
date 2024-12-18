using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Base.Logging;
using FileHelpers;
using Newtonsoft.Json;
using UnityEngine.Android;

namespace Base.Helper
{
    public static class FileUtilities
    {
        #region Save & Load

        public static bool HasFile(string path) => File.Exists(path);

        public static void DeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                File.Delete(path);
            }
        }
        
        public static void SaveToBinary(object data, string fileName)
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + fileName,
                FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);

                throw;
            }

            stream.Close();
        }

        public static void LoadFromBinary<T>(out T result, string filename)
        {
            string path = Application.persistentDataPath + "/" + filename;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                result = (T)formatter.Deserialize(stream);
                stream.Close();
            }
            else result = default(T);
        }

        public static void LoadFromBinary<T>(out List<T> result, string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                result = formatter.Deserialize(stream) as List<T>;
                stream.Close();
            }
            else result = default;
        }

        public static bool SaveToJson(object data, string filename)
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + filename, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(stream);
            try
            {
                string json = JsonConvert.SerializeObject(data);
                sw.WriteLine(json);
            }
            catch (Exception e)
            {
                return false;
            }

            sw.Close();
            stream.Close();

            return true;
        }

        public static bool LoadFromJson<T>(out T result, string filename)
        {
            string path = Application.persistentDataPath + "/" + filename;
            if (File.Exists(path))
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                string jsonStr = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(jsonStr);
                reader.Close();
                stream.Close();

                return true;
            }
            else
            {
                result = default;

                return false;
            }
        }

        public static bool LoadJsonAsset<T>(out T result, string filePath)
        {
            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);
                string jsonStr = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(jsonStr);
                reader.Close();
                stream.Close();

                return true;
            }

            result = default;

            return false;
        }

        public static void SaveJsonAsset(object value, string filePath, FileMode fileMode = FileMode.Create)
        {
            FileStream stream = new FileStream(filePath, fileMode, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            try
            {
                string jsonStr = JsonConvert.SerializeObject(value);
                writer.WriteLine(jsonStr);
                writer.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }

            writer.Close();
            stream.Close();
        }

        public static void SaveToTextFile(object data, string fileName)
        {
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.WriteLine((string)data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }

            sw.Close();
            fs.Close();
        }

        public static string LoadTextResource(string filePath)
        {
            try
            {
                var textResource = Resources.Load<TextAsset>(filePath);

                return textResource.text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return string.Empty;
            }
        }

        public static string LoadTextFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return string.Empty;
            }
        }

        public static void SaveTextFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static T LoadData<T>(string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) return default;

            if (File.Exists(filePath))
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                string base64string = Encoding.UTF8.GetString(bytes);
                string jsonData = Encoding.UTF8.GetString(Convert.FromBase64String(base64string));
                T data = JsonConvert.DeserializeObject<T>(jsonData);

                return data;
            }

            return default;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="key">The private key for encryption</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadDataWithEncrypted<T>(string filePath, string key)
        {
            if (String.IsNullOrEmpty(filePath)) return default;

            if (File.Exists(filePath))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    byte[] rawData = Encryption.Decrypt(bytes, key);
                    T data = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(rawData));

                    return data;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    throw;
                }
            }

            return default;
        }

        public static void SaveData<T>(string fileDirectory, string fileName, T data)
        {
            string filePath = fileDirectory + $"/{fileName}";

            if (String.IsNullOrEmpty(filePath)) return;

            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            string jsonData = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);
            string final = Convert.ToBase64String(bytes);
            File.WriteAllLines(filePath, new[] {final});
        }

        public static void SaveDataWithEncrypted<T>(string fileDirectory, string fileName, T data, string key)
        {
            try
            {
                string filePath = fileDirectory + $"/{fileName}";

                if (String.IsNullOrEmpty(filePath)) return;

                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                string jsonData = JsonConvert.SerializeObject(data);
                byte[] rawData = Encryption.Encrypt(jsonData, key);
                string final = Encoding.UTF8.GetString(rawData);
                File.WriteAllLines(filePath, new[] {final});
            }
            catch (Exception e)
            {
                PDebug.Error(e, e.Message);

                throw;
            }
        }

        #endregion

        #region File Helper

        public static T[] ReadFromCsvFile<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            FileHelperEngine<T> engine = new FileHelperEngine<T>();

            return engine.ReadFile(filePath);
        }
        
        public static List<T> ReadFromCsvData<T>(byte[] rawData) where T : class
        {
            string source = Encoding.UTF8.GetString(rawData);

            FileHelperEngine<T> engine = new FileHelperEngine<T>();

            return engine.ReadStringAsList(source);
        }
        
        public static List<T> ReadFromCsvData<T>(string data) where T : class
        {
            FileHelperEngine<T> engine = new FileHelperEngine<T>();

            return engine.ReadStringAsList(data);
        }

        #endregion
    }
}