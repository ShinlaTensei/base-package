#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: ReferenceData.cs
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// Class used to serialize a reference to an object within Unity by saving it's guid.
    /// </summary>
    [Serializable]
    public class ReferenceData : StringData
    {
        /// <summary>
        /// Backing field of <see cref="Guid"/> used for serialization
        /// </summary>
        [SerializeField] private string m_guid = string.Empty;
        
        /// <summary>
        /// The globally unique identifier of the object to reference
        /// </summary>
        public string Guid
        {
            get => m_guid;
            set => m_guid = value;
        }
        
        /// <summary>
        /// Empty constructor using default member value
        /// </summary>
        public ReferenceData() {}
        
        /// <summary>
        /// Constructor setting the members to the provided values.
        /// </summary>
        /// <param name="index">The data's unique index.</param>
        /// <param name="objectName">The data's name.</param>
        /// <param name="guid">The referenced object's guid.</param>
        public ReferenceData(int index, string objectName, string guid)
                : base(index, objectName)
        {
            Guid = guid;
        }
        
        /// <summary>
        /// Constructor copying the provided data's values.
        /// </summary>
        /// <param name="data">The data to copy.</param>
        public ReferenceData(ReferenceData data)
        {
            CopyData(data);
        }
        
        /// <summary>
        /// Copies the values of the provided data to this instance.
        /// </summary>
        /// <param name="data">The data to copy the values from.</param>
        public override void CopyData(StringData data)
        {
            base.CopyData(data);

            if (data is ReferenceData referenceData)
            {
                Guid = referenceData.Guid;
            }
        }

        public override int CompareTo(object obj)
        {
            ReferenceData compareData = obj as ReferenceData;
            return compareData == null ? 1 : string.Compare(ObjectName, compareData.ObjectName, StringComparison.Ordinal);
        }
    }
}