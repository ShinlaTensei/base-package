using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    /// A concrete subclass of the Unity UI `Graphic` class that just skips drawing.
    /// Useful for providing a raycast target without actually drawing anything.
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/None Drawing Graphic")]
    public class NoneDrawingGraphics : Graphic
    {
        /// <summary>
        /// Set material dirty
        /// </summary>
        public override void SetMaterialDirty()
        {
            return;
        }

        /// <summary>
        /// set vertices dirty
        /// </summary>
        public override void SetVerticesDirty()
        {
            return;
        }

        /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }
}