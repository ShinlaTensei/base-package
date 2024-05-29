using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Editor
{
    public static class AddressableUtility
    {
        private static AddressableAssetSettings m_addressableSettings;

        private static AddressableAssetSettings AddressableSettings
        {
            get
            {
                if (m_addressableSettings == null)
                {
                    string guid = AssetDatabase.FindAssets("t:AddressableAssetSettings").FirstOrDefault();
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError("Could not find Addressable settings");
                        return null;
                    }

                    m_addressableSettings = AssetDatabaseUtility.LoadAssetFromGuid<AddressableAssetSettings>(guid);

                    if (m_addressableSettings == null)
                    {
                        Debug.LogError("Could not find Addressable Settings");
                    }
                }

                return m_addressableSettings;
            }
        }
        
        /// <summary>
        /// Check if the object given with guid is an addressable
        /// </summary>
        public static bool IsAddressable(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }
            
            return AddressableSettings.FindAssetEntry(guid) != null;
        }
        
        /// <summary>
        /// Check if the given object is an addressable
        /// </summary>
        public static bool IsAddressable(Object asset)
        {
            if (asset == null)
            {
                return false;
            }
            return IsAddressable(AssetDatabaseUtility.GetAssetGuid(asset));
        }

        public static Object LoadAssetFromAddress(string address, Type objectType)
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError("Address is empty or null");
                return null;
            }

            AddressableAssetEntry assetEntry = default;
            foreach (var group in AddressableSettings.groups)
            {
                assetEntry = group.entries.Select(entry => entry).First(entry => entry.address.Equals(address));
            }

            return AssetDatabase.LoadAssetAtPath(AssetDatabase.AssetPathToGUID(assetEntry.address), objectType);
        }
        
        /// <summary>
        /// Used to fetch an asset's address in the editor
        /// </summary>
        public static string GetAddressFromObject(Object obj)
        {
            string guid = AssetDatabaseUtility.GetAssetGuid(obj);

            AddressableAssetEntry assetEntry = AddressableSettings.FindAssetEntry(guid);

            if (assetEntry == null)
            {
                Debug.LogError($"Asset is not an Addressable: \"{obj.name}\"!");
            }

            return assetEntry?.address;
        }
    }

}