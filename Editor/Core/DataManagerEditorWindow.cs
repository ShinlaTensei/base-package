using Base.Core;
using Sirenix.OdinInspector.Editor;

namespace Base.Editor
{
    public abstract class DataManagerEditorWindow<T> : OdinEditorWindow where T : IDataObject
    {
        /// <summary>
        /// Static reference to the window itself to avoid multiple memory allocations when opening and closing the window.
        /// </summary>
        protected static DataManagerEditorWindow<T> m_window;
    }
}