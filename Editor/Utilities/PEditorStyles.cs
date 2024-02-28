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

        private static readonly Color m_backgroundRedColor = new Color(1f, 0, 0, 1f);

        private static readonly Color m_backgroundGreenDark = new Color(0.1f, 0.75f, 0.03f, 1f);

        private static readonly Color m_defaultBackgroundColor = new Color(1f, 1f, 1f, 1f);

        private static readonly Color32 m_defaultCollectionHeaderColor = new Color32(61, 61, 61, 255);

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

        public static Color BackgroundRedColor => m_backgroundRedColor;

        public static Color BackgroundGreenDarkColor => m_backgroundGreenDark;

        public static Color DefaultBackgroundColor => m_defaultBackgroundColor;

        public static Color32 DefaultCollectionHeaderColor => m_defaultCollectionHeaderColor;
    }
}