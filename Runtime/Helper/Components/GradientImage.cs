using System;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    /// <summary>
    /// Draws a Image, similar to the regular Images. You can set four corner colors to have a nice gradient.
    /// </summary>
    [AddComponentMenu("Base Component/GradientImage", 14)]
    public class GradientImage : Image
    {
        // Notice:
        // A lot of the code is based upon Unity3d's Image component.
        // Some of it is inconsistent with our internal coding convention. In this case this is based upon Unitys Image script, or as a requirement by the implemented Interfaces
        // For reference please see https://github.com/tenpn/unity3d-ui/blob/master/UnityEngine.UI/UI/Core/Image.cs

        [SerializeField] private Color _additionalColor1 = Color.white;
        [SerializeField] private Color _additionalColor2 = Color.white;
        [SerializeField] private Color _additionalColor3 = Color.white;

        [Range(0, 1), SerializeField]

        private float _opacity = 1f;
        
        public override Texture mainTexture
        {
            get
            {
                if (sprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }

                    return s_WhiteTexture;
                }

                return sprite.texture;
            }
        }

        public override Material material
        {
            get
            {
                if (m_Material != null)
                {
                    return m_Material;
                }

                return defaultMaterial;
            }
            set { base.material = value; }
        }

        /// <summary>
        /// Whether the Image has a border to work with.
        /// </summary>
        private bool HasBorder
        {
            get
            {
                if (sprite != null)
                {
                    Vector4 v = sprite.border;
                    return v.sqrMagnitude > 0f;
                }

                return false;
            }
        }
        
        protected override void OnCanvasHierarchyChanged()
        {
            type = HasBorder ? Type.Sliced : Type.Simple;
            base.OnCanvasHierarchyChanged();
        }

        /// <summary>
        /// Generates the Mesh for the Canvas, and saves it into the VertexHelper
        /// </summary>
        /// <param name="vertexHelper">Helper that collects the new vertices</param>
        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            if (vertexHelper == null)
            {
                base.OnPopulateMesh(vertexHelper); // Well, Unity calls this function, so they surely must make sure this somehow works. Anyway not my problem.
                return;
            }

            if (HasBorder)
            {
                GenerateSlicedMesh(vertexHelper);
            }
            else
            {
                GenerateSimpleMesh(vertexHelper);
            }
        }

        /// <summary>
        /// Get the boundaries of the Rect
        /// </summary>
        /// <returns></returns>
        private Vector4 GetDrawingDimensions()
        {
            Rect r = rectTransform.rect;
            return new Vector4(
                r.x,
                r.y,
                r.x + r.width,
                r.y + r.height
            );
        }

        /// <summary>
        /// Generates the UI-Mesh for a non-sliced sprite
        /// </summary>
        /// <param name="vertexHelper"></param>
        private void GenerateSimpleMesh(VertexHelper vertexHelper)
        {
            Vector4 rectDimensions = GetDrawingDimensions();
            Vector4 uv = sprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;

            vertexHelper.Clear();
            Vector3[] positions = new Vector3[4];
            Color[] colors = GetCornerColors();
            Vector2[] uvs = new Vector2[positions.Length];

            positions[0] = new Vector2(rectDimensions.x, rectDimensions.y);
            positions[1] = new Vector2(rectDimensions.x, rectDimensions.w);
            positions[2] = new Vector2(rectDimensions.z, rectDimensions.w);
            positions[3] = new Vector2(rectDimensions.z, rectDimensions.y);

            uvs[0] = new Vector2(uv.x, uv.y);
            uvs[1] = new Vector2(uv.x, uv.w);
            uvs[2] = new Vector2(uv.z, uv.w);
            uvs[3] = new Vector2(uv.z, uv.y);

            vertexHelper.AddVert(positions[0], colors[0], uvs[0]);
            vertexHelper.AddVert(positions[1], colors[1], uvs[1]);
            vertexHelper.AddVert(positions[2], colors[2], uvs[2]);
            vertexHelper.AddVert(positions[3], colors[3], uvs[3]);

            vertexHelper.AddTriangle(0, 1, 2);
            vertexHelper.AddTriangle(2, 3, 0);
        }

        /// <summary>
        /// Generates the UI-Mesh for a sliced-sprite
        /// </summary>
        /// <param name="vertexHelper"></param>
        private void GenerateSlicedMesh(VertexHelper vertexHelper)
        {
            Sprite currentSprite = sprite;
            if (currentSprite == null)
            {
                GenerateSimpleMesh(vertexHelper);
                return;
            }
          
            // Uv Boundaries
            Vector4 outerUv = UnityEngine.Sprites.DataUtility.GetOuterUV(currentSprite);
            Vector4 innerUv = UnityEngine.Sprites.DataUtility.GetInnerUV(currentSprite);

            // Data used for determining the interpolation of Vertex-Colors and Fill-Sections on the inner borders
            Vector4 scaledBorder = currentSprite.border / multipliedPixelsPerUnit;
            Rect rect = rectTransform.rect;
            float relativeLeftBorder = scaledBorder.x / rect.width;
            float relativeRightBorder = (rect.width - scaledBorder.z) / rect.width;
            float relativeBottomBorder = scaledBorder.y / rect.height;
            float relativeTopBorder = (rect.height - scaledBorder.w) / rect.height;
            
            vertexHelper.Clear();

            /*
             * Vertex-Layout is ordered as follows:
             *
             *  04 - 05      -     11 - 08
             *   | \  |      /      | /  |
             *  07 - 06      -     10 - 09
             *
             *   | /  |      /      | /  |
             *
             *  01 - 02      -     14 - 15
             *   | /  |      /      | \  |
             *  00 - 03      -     13 - 12
            */

            // Get the Vertex-Data
            Vector3[] vertices = GetVerticesForSliced(scaledBorder);
            Vector2[] uvs = GetUvsForSliced(outerUv, innerUv);

            Color[] vertexColors = GetVertexColorsForSliced(GetCornerColors(),
                relativeLeftBorder, relativeRightBorder, relativeBottomBorder, relativeTopBorder);

            for (int i = 0; i < vertices.Length; ++i)
            {
                vertexHelper.AddVert(vertices[i], vertexColors[i], uvs[i]);
            }

            // Add Indices
            AddIndicesForSliced(vertexHelper);
        }

        /// <summary>
        /// Get the Vertex Colors for a SlicedSprite
        /// </summary>
        /// <returns></returns>
        private Color[] GetVertexColorsForSliced(Color[] cornerColors,
            float relativeLeftBorder,
            float relativeRightBorder,
            float relativeBottomBorder,
            float relativeTopBorder)
        {
            Color[] vertexColors = new Color[16];

            //outer corners
            vertexColors[0] = cornerColors[0];
            vertexColors[4] = cornerColors[1];

            vertexColors[8] = cornerColors[2];
            vertexColors[12] = cornerColors[3];

            vertexColors[3] = Color.Lerp(cornerColors[0], cornerColors[3], relativeLeftBorder);
            vertexColors[5] = Color.Lerp(cornerColors[1], cornerColors[2], relativeLeftBorder);

            vertexColors[13] = Color.Lerp(cornerColors[0], cornerColors[3], relativeRightBorder);
            vertexColors[11] = Color.Lerp(cornerColors[1], cornerColors[2], relativeRightBorder);

            //Outer boders left/right
            vertexColors[1] = Color.Lerp(cornerColors[0], cornerColors[1], relativeBottomBorder);
            vertexColors[15] = Color.Lerp(vertexColors[12], vertexColors[8], relativeBottomBorder);
            vertexColors[7] = Color.Lerp(cornerColors[0], cornerColors[1], relativeTopBorder);
            vertexColors[9] = Color.Lerp(vertexColors[12], vertexColors[8], relativeTopBorder);

            //inner corners
            vertexColors[2] = Color.Lerp(vertexColors[3], vertexColors[5], relativeBottomBorder);
            vertexColors[6] = Color.Lerp(vertexColors[3], vertexColors[5], relativeTopBorder);
            vertexColors[14] = Color.Lerp(vertexColors[13], vertexColors[11], relativeBottomBorder);
            vertexColors[10] = Color.Lerp(vertexColors[13], vertexColors[11], relativeTopBorder);
            return vertexColors;
        }

        /// <summary>
        /// Get the Uv coordinates for a SlicedSprite
        /// </summary>
        /// <param name="outerUv">The boundings of the outer Uvs</param>
        /// <param name="innerUv">The boundings of the inner Uvs</param>
        /// <returns></returns>
        private Vector2[] GetUvsForSliced(Vector4 outerUv, Vector4 innerUv)
        {
            Vector2[] uvs = new Vector2[16];

            // outer left edge
            uvs[0] = new Vector2(outerUv.x, outerUv.y);
            uvs[1] = new Vector2(outerUv.x, innerUv.y);
            uvs[4] = new Vector2(outerUv.x, outerUv.w);
            uvs[7] = new Vector2(outerUv.x, innerUv.w);

            uvs[2] = new Vector2(innerUv.x, innerUv.y);
            uvs[3] = new Vector2(innerUv.x, outerUv.y);
            uvs[5] = new Vector2(innerUv.x, outerUv.w);
            uvs[6] = new Vector2(innerUv.x, innerUv.w);

            uvs[10] = new Vector2(innerUv.z, innerUv.w);
            uvs[11] = new Vector2(innerUv.z, outerUv.w);
            uvs[13] = new Vector2(innerUv.z, outerUv.y);
            uvs[14] = new Vector2(innerUv.z, innerUv.y);

            uvs[8] = new Vector2(outerUv.z, outerUv.w);
            uvs[9] = new Vector2(outerUv.z, innerUv.w);
            uvs[12] = new Vector2(outerUv.z, outerUv.y);
            uvs[15] = new Vector2(outerUv.z, innerUv.y);
            return uvs;
        }

        /// <summary>
        /// Gets the Vertices for a SlicedSprite.
        /// </summary>
        /// <returns>The Vertices for a SlicedSprite</returns>
        private Vector3[] GetVerticesForSliced(Vector4 scaledBorder)
        {
            // the boundaries of the rectTransforms rect

            //XY: BL, ZW TR
            Vector4 outerCoordinates = GetDrawingDimensions();
            Vector4 innerCoordinates = outerCoordinates + new Vector4(scaledBorder.x, scaledBorder.y, -scaledBorder.z, -scaledBorder.w);

            Vector3[] vertices = new Vector3[16];

            // Calculating the Vertex positions from the generated Edges

            //Positions
            vertices[0] = new Vector3(outerCoordinates.x, outerCoordinates.y);
            vertices[1] = new Vector3(outerCoordinates.x, innerCoordinates.y);
            vertices[2] = new Vector3(innerCoordinates.x, innerCoordinates.y);
            vertices[3] = new Vector3(innerCoordinates.x, outerCoordinates.y);

            vertices[4] = new Vector3(outerCoordinates.x, outerCoordinates.w);
            vertices[5] = new Vector3(innerCoordinates.x, outerCoordinates.w);
            vertices[6] = new Vector3(innerCoordinates.x, innerCoordinates.w);
            vertices[7] = new Vector3(outerCoordinates.x, innerCoordinates.w);

            vertices[8] = new Vector3(outerCoordinates.z, outerCoordinates.w);
            vertices[9] = new Vector3(outerCoordinates.z, innerCoordinates.w);
            vertices[10] = new Vector3(innerCoordinates.z, innerCoordinates.w);
            vertices[11] = new Vector3(innerCoordinates.z, outerCoordinates.w);

            vertices[12] = new Vector3(outerCoordinates.z, outerCoordinates.y);
            vertices[13] = new Vector3(innerCoordinates.z, outerCoordinates.y);
            vertices[14] = new Vector3(innerCoordinates.z, innerCoordinates.y);
            vertices[15] = new Vector3(outerCoordinates.z, innerCoordinates.y);

            return vertices;
        }

        /// <summary>
        /// Build the Triangles for a Sliced Image
        /// </summary>
        /// <param name="vertexHelper"></param>
        /// <param name="currenFillState"></param>
        private void AddIndicesForSliced(VertexHelper vertexHelper)
        {
            vertexHelper.AddTriangle(0, 1, 2); //BL
            vertexHelper.AddTriangle(2, 3, 0);

            vertexHelper.AddTriangle(1, 7, 6); //L
            vertexHelper.AddTriangle(6, 2, 1);

            vertexHelper.AddTriangle(4, 5, 6); //TL
            vertexHelper.AddTriangle(6, 7, 4);

            vertexHelper.AddTriangle(5, 11, 10); //T
            vertexHelper.AddTriangle(10, 6, 5);

            if (fillCenter) // C
            {
                vertexHelper.AddTriangle(2, 6, 10);
                vertexHelper.AddTriangle(10, 14, 2);
            }

            vertexHelper.AddTriangle(2, 14, 13); //B
            vertexHelper.AddTriangle(13, 3, 2);

            vertexHelper.AddTriangle(8, 9, 10); //TR
            vertexHelper.AddTriangle(10, 11, 8);

            vertexHelper.AddTriangle(9, 15, 14); //R
            vertexHelper.AddTriangle(14, 10, 9);

            vertexHelper.AddTriangle(12, 13, 14); //BR
            vertexHelper.AddTriangle(14, 15, 12);
        }

        /// <summary>
        /// Checks whether the a Raycast is inside the shape. The Raycast is usually only performed when it is inside the rect.
        /// </summary>
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return true;
        }

        // Get the corner colors
        private Color[] GetCornerColors()
        {
            Color color0 = color;
            Color color1 = _additionalColor1;
            Color color2 = _additionalColor2;
            Color color3 = _additionalColor3;

            color0.a *= _opacity;
            color1.a *= _opacity;
            color2.a *= _opacity;
            color3.a *= _opacity;

            return new[] {color0, color1, color2, color3};
        }

        /// <summary>
        /// Updates the color in a specific corner of the gradient image
        /// </summary>
        /// <param name="position">The position to update</param>
        /// <param name="c">The color to update</param>
        /// <param name="applyAlpha">If the alpha value of the parameter color should be applied</param>
        public void UpdateColor(Position position, Color c, bool applyAlpha)
        {
            ref Color colorRef = ref c;
            switch (position)
            {
                case Position.TopLeft:
                {
                    colorRef = ref _additionalColor1;
                    break;
                }
                case Position.TopRight:
                {
                    colorRef = ref _additionalColor2;
                    break;
                }
                case Position.BottomLeft:
                {
                    //Special case because the color property of the base class Image is used
                    if (!applyAlpha)
                    {
                        c.a = color.a;
                    }

                    color = c;
                    return;
                }
                case Position.BottomRight:
                {
                    colorRef = ref _additionalColor3;
                    break;
                }
            }

            if (!applyAlpha)
            {
                c.a = colorRef.a;
            }

            colorRef = c;
        }

        /// <summary>
        /// The position in the image, int values important, used in <see cref="Chimera.MantiCore.UI.Widgets.GradientImage"/> for array access!
        /// </summary>
        public enum Position
        {
            TopLeft = 0,
            TopRight = 1,
            BottomLeft = 2,
            BottomRight = 3
        }
    }
}
