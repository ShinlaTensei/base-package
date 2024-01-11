#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: WorkingCopyCollection.cs
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// Class representing a collection with a separate working copy to allow reverting.
    /// The working copy is shallow, meaning list element instances are shared between both lists.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    [Serializable]
    public class WorkingCopyCollection<T>
    {
        [SerializeField] private List<T> m_data;

        private List<T> m_workingCopy;

        public List<T> Data
        {
            get => m_data;
            set => m_data = value;
        }

        public List<T> WorkingCopy
        {
            get
            {
                if (m_workingCopy == null)
                {
                    CreateWorkingCopy();
                }

                return m_workingCopy;
            }
            set => m_workingCopy = value;
        }

        public void CreateWorkingCopy()
        {
            m_workingCopy = new List<T>();
            if (m_data == null)
            {
                return;
            }

            foreach (T value in m_data)
            {
                m_workingCopy.Add(GetWorkingCopy(value));
            }
        }
        
        public void Apply()
        {
            Data = new List<T>();
            foreach (T value in WorkingCopy)
            {
                Data.Add(GetDataCopy(value));
            }
        }

        public void Revert()
        {
            CreateWorkingCopy();
        }
        
        public virtual T GetWorkingCopy(T original)
        {
            return original;
        }

        public virtual T GetDataCopy(T original)
        {
            return original;
        }
    }
    
    /// <summary>
    /// Class representing a collection with a separate working copy to allow reverting.
    /// The working copy is shallow, meaning list element instances are shared between both lists.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    [Serializable]
    public class DeepWorkingCopyCollection<T> : WorkingCopyCollection<T> where T : ICloneable
    {
        public override T GetWorkingCopy(T original)
        {
            return (T)original.Clone();
        }

        public override T GetDataCopy(T original)
        {
            return (T)original.Clone();
        }
    }
}