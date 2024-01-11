#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: DataContainer.cs
#endregion

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// Base class for data containers to be serialized and managed by an <c>Editor Window</c>
    /// </summary>
    /// <typeparam name="T">The specific type of the contained data.</typeparam>
    [Serializable]
    public abstract class DataContainer<T> : ScriptableObject where T : IDataObject
    {
        
    }
}