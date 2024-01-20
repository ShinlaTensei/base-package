#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: StringData.cs
#endregion

using System;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// Class used to serialize and compare objects with a given index, name and type.
    /// </summary>
    [Serializable]
    public class StringData : IDataObject, IComparable
    {
        /// <summary>
        /// Backing field of <see cref="Index"/> used for serialization.
        /// </summary>
        /// 
        [SerializeField] private int    m_index      = 0;
        /// <summary>
        /// Backing field of <see cref="ObjectName"/> used for serialization.
        /// </summary>
        /// 
        [SerializeField] private string m_objectName = string.Empty;
        /// <summary>
        /// Backing field of <see cref="Type"/> used for serialization.
        /// </summary>
        /// 
        [SerializeField] private string m_type       = string.Empty;

        /// <summary>
        /// The data's unique index used to reference it.
        /// </summary>
        public int Index
        {
            get => m_index;
            set => m_index = value;
        }

        /// <summary>
        /// The data's type used for filtering it in the user interface.
        /// </summary>
        public string Type
        {
            get => m_type;
            set => m_type = value;
        }

        /// <summary>
        /// The data's name displayed in the user interface.
        /// </summary>
        public string ObjectName
        {
            get => m_objectName;
            set => m_objectName = value;
        }
        /// <summary>
        /// Empty constructor with default member values
        /// </summary>
        public StringData() {}
        
        /// <summary>
        /// Constructor setting the members to the provided values.
        /// </summary>
        /// <param name="index">The data's unique index.</param>
        /// <param name="objectName">The data's name.</param>
        /// <param name="type">The data's type used for filtering in the user interface.</param>
        public StringData(int index, string objectName, string type = "")
        {
            Index      = index;
            ObjectName = objectName;
            Type       = type;
        }
        
        /// <summary>
        /// Constructor copying the provided data's value
        /// </summary>
        /// <param name="data"></param>
        public StringData(StringData data)
        {
            CopyData(data);
        }
        
        /// <summary>
        /// Copies the values of the provided data to this instance.
        /// </summary>
        /// <param name="data">The data to copy the values from.</param>
        public virtual void CopyData(StringData data)
        {
            Index      = data.Index;
            ObjectName = data.ObjectName;
            Type       = data.Type;
        }
        
        /// <summary>
        /// <see cref="IComparable"/> implementation comparing two instances by comparing their names.
        /// </summary>
        /// <param name="obj">The data to compare to.</param>
        /// <returns>
        /// <c>-1</c> if the provided object precedes this instance;
        /// <c>0</c> if they have the same sort order;
        /// <c>1</c> if the provided object follows this instance or it is <c>null</c>
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            StringData compareData = obj as StringData;
            return compareData == null ? 1 : string.Compare(ObjectName, compareData.ObjectName, StringComparison.Ordinal);
        }
    }
}