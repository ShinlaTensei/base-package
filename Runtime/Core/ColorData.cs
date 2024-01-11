#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: ColorData.cs
#endregion

using System;
using UnityEngine;

namespace Base.Core
{
    /// <summary>
    /// The data necessary to saved for a global color
    /// </summary>
    [Serializable]
    public class ColorData : StringData
    {
        /// <summary>
        /// Backing field for <see cref="Color"/> used for serialization.
        /// </summary>
        [SerializeField]
        private Color32 m_color = UnityEngine.Color.white;

        /// <summary>
        /// The color contained in the data to be applied all across the application using a <see cref="ColorBinding"/>.
        /// </summary>
        public Color32 Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        /// <summary>
        /// Empty constructor leaving the data's fields at their default values.
        /// Calls the base constructor.
        /// </summary>
        public ColorData() { }

        /// <summary>
        /// Empty constructor leaving the data's fields at their default values.
        /// Calls the base constructor.
        /// </summary>
        /// <param name="index">The unique index assigned to the data.</param>
        /// <param name="objectName">The data's name to be displayed in the user interface.</param>
        public ColorData(int index, string objectName) : base(index, objectName) { }

        /// <summary>
        /// Constructor copying the data of another instance to this one.
        /// </summary>
        /// <param name="data">The data to copy all values from.</param>
        public ColorData(ColorData data)
        {
            CopyData(data);
        }

        /// <summary>
        /// Copies the data of the provided instance to this one.
        /// </summary>
        /// <param name="data">The data to copy all values from.</param>
        public override void CopyData(StringData data)
        {
            base.CopyData(data);

            ColorData colorData = data as ColorData;
            if (colorData == null)
            {
                return;
            }

            Color = colorData.Color;
        }
    }
}