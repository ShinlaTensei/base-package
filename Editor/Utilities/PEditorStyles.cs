#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: PEditorStyles.cs
#endregion

using UnityEngine;

namespace Base.Editor
{
    public class PEditorStyles
    {
        /// <summary>
        /// Backing field for <see cref="BackgroundColorGrey"/>.
        /// </summary>
        private static readonly Color32 m_backgroundColorGrey = new Color32(255, 255, 255, 10);

        /// <summary>
        /// Backing field for <see cref="SeparatorColorBlack"/>.
        /// </summary>
        private static readonly Color32 m_separatorColorBlack = new Color32(0, 0, 0, 75);

        /// <summary>
        /// Backing field for <see cref="SeparatorColorLightGrey"/>.
        /// </summary>
        private static readonly Color32 m_separatorColorLightGrey = new Color32(255, 255, 255, 50);

        /// <summary>
        /// Grey Color, mostly used for Window Manager Backgrounds.
        /// </summary>
        public static Color32 BackgroundColorGrey => m_backgroundColorGrey;

        /// <summary>
        /// Black Color, mostly used in combination with <see cref="SeparatorColorLightGrey"/> to show a shadowed separation line.
        /// </summary>
        public static Color32 SeparatorColorBlack => m_separatorColorBlack;

        /// <summary>
        /// Black Color, mostly used in combination with <see cref="SeparatorColorBlack"/> to show a shadowed separation line.
        /// </summary>
        public static Color32 SeparatorColorLightGrey => m_separatorColorLightGrey;
    }
}