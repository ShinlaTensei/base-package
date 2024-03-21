#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioDataContainer.cs
#endregion

using System.Collections.Generic;
using Base.Core;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace Base
{
    [CreateAssetMenu(menuName = "Test")]
    public class AudioDataContainer : DataContainer<AudioAssetData>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The serialized asset types used to fill <see cref="DataCollection"/>
        /// </summary>
        [SerializeField] private List<string> m_serializedAssetTypes;
        /// <summary>
        /// The serialized assets used to fill <see cref="DataCollection"/>
        /// </summary>
        [SerializeField] private List<string> m_serializedAssets;
        /// <summary>
        /// Backing field for <see cref="AudioTypes"/>
        /// </summary>
        [SerializeField] private List<string> m_audioTypes;
        /// <summary>
        /// Backing field for <see cref="DataCollection"/>
        /// </summary>
        private List<AudioAssetData> m_audioAssets;
        /// <summary>
        /// Backing field for <see cref="WorkingCopy"/>
        /// </summary>
        private List<AudioAssetData> m_audioAssetWorkingCopy;
        /// <summary>
        /// Backing field of <see cref="AudioSetting"/>
        /// </summary>
        public List<string> AudioTypes
        {
            get => m_audioTypes;
            set => m_audioTypes = value;
        }

        public override List<AudioAssetData> DataCollection
        {
            get => m_audioAssets;
            protected set => m_audioAssets = value;
        }

        public override List<AudioAssetData> WorkingCopy
        {
            get
            {
                if (m_audioAssetWorkingCopy == null)
                {
                    CreateWorkingCopy();
                }

                return m_audioAssetWorkingCopy;
            }
            protected set => m_audioAssetWorkingCopy = value;
        }

        public void OnBeforeSerialize()
        {
            m_serializedAssetTypes = new List<string>();
            m_serializedAssets = new List<string>();
        
            if (DataCollection == null)
            {
                return;
            }
        
            foreach (AudioAssetData data in DataCollection)
            {
                m_serializedAssetTypes.Add(data.GetType().FullName);
                m_serializedAssets.Add(JsonConvert.SerializeObject(data));
            }
        }
        
        public void OnAfterDeserialize()
        {
            DataCollection = new List<AudioAssetData>();
            for (int i = 0; i < m_serializedAssets.Count; i++)
            {
                Type type = GetType(m_serializedAssetTypes[i]);
                DataCollection.Add(JsonConvert.DeserializeObject(m_serializedAssets[i], type) as AudioAssetData);
            }
        }

        public override void Save()
        {
#if UNITY_EDITOR
            
#endif
            
            base.Save();
        }

        /// <summary>
        /// Fetches the type from the loaded assemblies based on it's fully qualified name.
        /// </summary>
        /// <param name="typeName">The type's fully qualified name.</param>
        /// <returns><c>null</c> if no matching type was found, otherwise the matching type.</returns>
        private Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}