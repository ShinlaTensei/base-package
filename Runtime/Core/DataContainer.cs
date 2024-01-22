#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: DataContainer.cs
#endregion

using System;
using System.Collections.Generic;
using Base.Services;
using UnityEditor;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// Base class for data containers to be serialized and managed by an <c>Editor Window</c>
    /// </summary>
    /// <typeparam name="T">The specific type of the contained data.</typeparam>
    [Serializable]
    public abstract class DataContainer<T> : ScriptableObject where T : class, IDataObject
    {
        /// <summary>
        /// The collection hold the serialized data
        /// </summary>
        public abstract List<T> DataCollection { get; protected set; }
        /// <summary>
        /// The working copy data to freely modified.
        /// </summary>
        public abstract List<T> WorkingCopy { get; protected set; }
        
        public abstract Dictionary<string, IBaseSetting> AudioSettingMap { get; protected set; }

        /// <summary>
        /// The dictionary of all data keyed to their names for quick consecutive access.
        /// </summary>
        protected IDictionary<string, T> m_dataDictionary;
        
        /// <summary>
        /// Creates the internal dictionary of data keyed to its name for consecutive access.
        /// </summary>
        /// <param name="dataCollection"></param>
        protected virtual void CreateDictionary(List<T> dataCollection)
        {
            m_dataDictionary = new Dictionary<string, T>();
            if (dataCollection is null)
            {
                return;
            }
            for (int i = 0; i < dataCollection.Count; i++)
            {
                if (m_dataDictionary.ContainsKey(dataCollection[i].ObjectName))
                {
                    continue;
                }

                m_dataDictionary[dataCollection[i].ObjectName] = dataCollection[i];
            }
        }
        
        /// <summary>
        /// Determines if a data entry matching the specified identifier exists in the internal data dictionary.
        /// Initializes the data dictionary if it has not been set up yet for quick consecutive access.
        /// </summary>
        /// <param name="identifier">The identifier to search the matching data for.</param>
        /// <returns><c>true</c> if matching data was found, otherwise <c>false</c>.</returns>
        public bool Contains(string identifier)
        {
            if (m_dataDictionary == null)
            {
                CreateDictionary(DataCollection);
            }

            return m_dataDictionary?.ContainsKey(identifier) ?? false;
        }
        
        /// <summary>
        /// Tries to fetch the data matching the specified identifier from the internal data dictionary.
        /// Initializes the data dictionary if it has not been set up yet for quick consecutive access.
        /// </summary>
        /// <param name="identifier">The identifier to return the matching data for.</param>
        /// <param name="data">The variable to point to the correct data.</param>
        /// <returns><c>true</c> if matching data was found, otherwise <c>false</c>.</returns>
        public bool TryGetData(string identifier, out T data)
        {
            if (m_dataDictionary == null)
            {
                CreateDictionary(DataCollection);
            }

            return m_dataDictionary.TryGetValue(identifier, out data);
        }
        
        /// <summary>
        /// Overrides the currently serialized data with the working copy and serializes the data.
        /// </summary>
        public virtual void Save()
        {
#if UNITY_EDITOR
            if (WorkingCopy == null)
            {
                return;
            }

            List<T> newCollection = new List<T>();
            foreach (T data in WorkingCopy)
            {
                newCollection.Add(GetDataCopy(data));
            }

            newCollection.Sort((one, two) => string.Compare(one.ObjectName, two.ObjectName, StringComparison.Ordinal));

            DataCollection = newCollection;
            WorkingCopy = null;
            m_dataDictionary = null;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
#endif
        }
        
        /// <summary>
        /// Creates a deep copy of the currently serialized list of data and saves it in the working copy.
        /// </summary>
        public virtual void CreateWorkingCopy()
        {
            WorkingCopy = new List<T>();
            if (DataCollection == null)
            {
                return;
            }

            foreach (T data in DataCollection)
            {
                WorkingCopy.Add(GetDataCopy(data));
            }
        }

        /// <summary>
        /// Recreates the working copy from the currently serialized data, discarding all changes made to it.
        /// </summary>
        public virtual void Revert()
        {
            CreateWorkingCopy();
        }
        
        /// <summary>
        /// Creates a copy of the specified data.
        /// </summary>
        /// <param name="original">The data to create a copy of.</param>
        /// <returns>A new instance copied from the provided data.</returns>
        public virtual T GetDataCopy(object original)
        {
            return (T) Activator.CreateInstance(original.GetType(), original);
        }
    }
}