#region Header
// Date: 12/10/2023
// Created by: Huynh Phong Tran
// File name: DistortedImage.cs
#endregion

using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    /// <summary>
    /// Draws a Image, similar to the regular Images. It has offsets for each corner, so more complex, distorted images are possible, without adding new graphics.
    /// </summary>
    [AddComponentMenu("UI/DistortedImage", 13)]
    public class DistortedImage : Image
    {
        // Notice:
        // A lot of the code is based upon Unity3d's Image component.
        // Some of it is inconsistent with our internal coding convention. In this case this is based upon Unitys Image script, or as a requirement by the implemented Interfaces
        // For reference please see https://github.com/tenpn/unity3d-ui/blob/master/UnityEngine.UI/UI/Core/Image.cs

        // In which area of a sliced sprite are we currently
        private enum FillState
        {
            Empty = 0,
            LeftSection,
            MidSection,
            RightSection,
            Full
        }

        private enum FillDirection
        {
            LeftToRight = 0,
            TopToBottom = 1,
            RightToLeft = 2,
            BottomToTop = 3
        }

        private enum ColorGradientType
        {
            Solid,
            GradientLeftToRight,
            GradientTopToBottom,
            Vertices
        }

        protected static Material _etc1DefaultUI = null;



        [SerializeField] private ColorGradientType _colorFillType = ColorGradientType.Solid;
        [SerializeField] private Color _additionalColor1 = Color.white;
        [SerializeField] private Color _additionalColor2 = Color.white;
        [SerializeField] private Color _additionalColor3 = Color.white;
        [Range(0, 1)][SerializeField] private float _opacity = 1f;

        [SerializeField] private bool _fill = false;

        private Line2d _calculatedLine0;
        private Line2d _calculatedLine1;
        private Line2d _calculatedLine2;
        private Line2d _calculatedLine3;

        public float FillAmount
        {
            get => fillAmount;
            set
            {
                SetAllDirty();
                fillAmount = value;
            }
        }

        [SerializeField] private FillDirection _fillDirection = FillDirection.LeftToRight;

        [SerializeField] private Vector2 _offsetBottomLeft = default;
        [SerializeField] private Vector2 _offsetTopLeft = default;
        [SerializeField] private Vector2 _offsetTopRight = default;
        [SerializeField] private Vector2 _offsetBottomRight = default;

        [SerializeField] private float _leftFrame = 0f;
        [SerializeField] private float _topFrame = 0f;
        [SerializeField] private float _rightFrame = 0f;
        [SerializeField] private float _bottomFrame = 0f;

#if UNITY_EDITOR
        /// <summary>
        /// Makes sure the lines are calculated whenever values change in editor or whenever else needed
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            CalculateLines();

        }
#endif

        /// <summary>
        /// Sets the colors of the corner
        /// </summary>
        /// <param name="colors">The colors that are set starting bottom-left going clockwise. Sets 1 or 4 Colors</param>
        public void SetCornerColors(Color[] colors)
        {
            if (colors.Length == 0)
            {
                Debug.LogError("Setting Corner Colors failed on  Object \"" + gameObject.name + "\"", this);
                return;
            }

            color = colors[0];
            if (colors.Length >= 4)
            {
                _additionalColor1 = colors[1];
                _additionalColor2 = colors[2];
                _additionalColor3 = colors[3];
            }

            SetVerticesDirty();
        }

        /// <summary>
        /// Makes sure the lines are calculated in the beginning
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            CalculateLines();
        }

        /// <summary>
        /// Makes sure the lines are calculated when the Rect size has changed
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            CalculateLines();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();
            CalculateLines();
        }

        /// <summary>
        /// Caches the lines that define the images outside boundaries.
        /// </summary>
        private void CalculateLines()
        {
            var rectDimensions = GetDrawingDimensions();
            var cornerPoints = new Vector2[4];
            cornerPoints[0] = new Vector2(rectDimensions.x, rectDimensions.y) + _offsetBottomLeft;
            cornerPoints[1] = new Vector2(rectDimensions.x, rectDimensions.w) + _offsetTopLeft;
            cornerPoints[2] = new Vector2(rectDimensions.z, rectDimensions.w) + _offsetTopRight;
            cornerPoints[3] = new Vector2(rectDimensions.z, rectDimensions.y) + _offsetBottomRight;

            _calculatedLine0 = new Line2d(cornerPoints[0], cornerPoints[1]);
            _calculatedLine1 = new Line2d(cornerPoints[1], cornerPoints[2]);
            _calculatedLine2 = new Line2d(cornerPoints[2], cornerPoints[3]);
            _calculatedLine3 = new Line2d(cornerPoints[3], cornerPoints[0]);
            _calculatedLine0.Distance += _leftFrame;
            _calculatedLine1.Distance += _topFrame;
            _calculatedLine2.Distance += _rightFrame;
            _calculatedLine3.Distance += _bottomFrame;
        }

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public static Material DefaultEtc1GraphicMaterial
        {
            get
            {
                if (_etc1DefaultUI == null) _etc1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();

                return _etc1DefaultUI;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (sprite == null)
                {
                    if (material != null && material.mainTexture != null) return material.mainTexture;

                    return s_WhiteTexture;
                }

                return sprite.texture;
            }
        }

        public override Material material
        {
            get
            {
                if (m_Material != null) return m_Material;

                return defaultMaterial;
            }
            set => base.material = value;
        }

        /// <summary>
        /// Whether the Image has a border to work with.
        /// </summary>
        public bool HasBorder
        {
            get
            {
                if (sprite != null)
                {
                    var v = sprite.border;
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
        /// <param name="toFill">Helper that collects the new vertices</param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (toFill == null)
            {
                base.OnPopulateMesh(toFill); // Well, Unity calls this function, so they surely must make sure this somehow works. Anyway not my problem.
                return;
            }

            if (HasBorder)
                GenerateSlicedMesh(toFill);
            else
                GenerateSimpleMesh(toFill);
        }

        /// <summary>
        /// Get the boundaries of the Rect
        /// </summary>
        /// <returns></returns>
        private Vector4 GetDrawingDimensions()
        {
            var r = rectTransform.rect;
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
            var rectDimensions = GetDrawingDimensions();
            var uv = sprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;

            var currentFillAmount = fillAmount;

            vertexHelper.Clear();
            if (_fill && fillAmount < Mathf.Epsilon) return;

            var positions = new Vector3[4];
            var colors = GetCornerColors();
            var uvs = new Vector2[positions.Length];

            positions[0] = new Vector2(rectDimensions.x, rectDimensions.y) + _offsetBottomLeft;
            positions[1] = new Vector2(rectDimensions.x, rectDimensions.w) + _offsetTopLeft;
            positions[2] = new Vector2(rectDimensions.z, rectDimensions.w) + _offsetTopRight;
            positions[3] = new Vector2(rectDimensions.z, rectDimensions.y) + _offsetBottomRight;

            uvs[0] = new Vector2(uv.x, uv.y);
            uvs[1] = new Vector2(uv.x, uv.w);
            uvs[2] = new Vector2(uv.z, uv.w);
            uvs[3] = new Vector2(uv.z, uv.y);

            if (_fill)
            {
                var offset = (int) _fillDirection;

                var o0 = offset;
                var o1 = Mod4Offset(1, offset);
                var o2 = Mod4Offset(2, offset);
                var o3 = Mod4Offset(3, offset);

                vertexHelper.AddVert(positions[o0], colors[o0], uvs[o0]);
                vertexHelper.AddVert(positions[o1], colors[o1], uvs[o1]);
                vertexHelper.AddVert(Vector3.Lerp(positions[o1], positions[o2], currentFillAmount), Color.Lerp(colors[o1], colors[o2], currentFillAmount),
                    Vector2.Lerp(uvs[o1], uvs[o2], currentFillAmount));
                vertexHelper.AddVert(Vector3.Lerp(positions[o0], positions[o3], currentFillAmount), Color.Lerp(colors[o0], colors[o3], currentFillAmount),
                    Vector2.Lerp(uvs[o0], uvs[o3], currentFillAmount));
            }
            else
            {
                vertexHelper.AddVert(positions[0], colors[0], uvs[0]);
                vertexHelper.AddVert(positions[1], colors[1], uvs[1]);
                vertexHelper.AddVert(positions[2], colors[2], uvs[2]);
                vertexHelper.AddVert(positions[3], colors[3], uvs[3]);
            }

            vertexHelper.AddTriangle(0, 1, 2);
            vertexHelper.AddTriangle(2, 3, 0);
        }

        private int Mod4Offset(int i, int offset)
        {
            return (i + offset) % 4;
        }

        /// <summary>
        /// Generates the UI-Mesh for a sliced-sprite
        /// </summary>
        /// <param name="vertexHelper"></param>
        private void GenerateSlicedMesh(VertexHelper vertexHelper)
        {
            var sprite = Sprite;

            if (sprite == null)
            {
                GenerateSimpleMesh(vertexHelper);
                return;
            }

            if (_fill && fillAmount < Mathf.Epsilon)
            {
                vertexHelper.Clear();
                return;
            }

            // Uv Boundaries
            var outerUv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            var innerUv = UnityEngine.Sprites.DataUtility.GetInnerUV(sprite);

            // Data used for determining the interpolation of Vertex-Colors and Fill-Sections on the inner borders
            var scaledBorder = sprite.border / multipliedPixelsPerUnit;
            var rect = rectTransform.rect;
            var relativeLeftBorder = scaledBorder.x / rect.width;
            var relativeRightBorder = (rect.width - scaledBorder.z) / rect.width;
            var relativeBottomBorder = scaledBorder.y / rect.height;
            var relativeTopBorder = (rect.height - scaledBorder.w) / rect.height;

            // Get key variables for the fill calculation
            var currentFillState = GetCurrentFillState(relativeLeftBorder, relativeRightBorder);
            var interpolationValue = GetInterpolationValue(currentFillState, relativeLeftBorder, relativeRightBorder);

            vertexHelper.Clear();

            /*
             * Vertex-Layout is ordered as followes:
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
            var vertices = GetVerticesForSliced(currentFillState, interpolationValue);
            var uvs = GetUvsForSliced(outerUv, innerUv, currentFillState, interpolationValue);

            var vertexColors = GetVertexColorsForSliced(GetCornerColors(), currentFillState, interpolationValue,
                                                        relativeLeftBorder, relativeRightBorder, relativeBottomBorder, relativeTopBorder);

            for (var i = 0; i < vertices.Length; ++i) vertexHelper.AddVert(vertices[i], vertexColors[i], uvs[i]);

            // Add Indices
            AddIndicesForSliced(vertexHelper, currentFillState);
        }

        /// <summary>
        /// Gets in which section of the slicedSprite the current fill value is
        /// </summary>
        /// <param name="leftBorder"></param>
        /// <param name="rightBorder"></param>
        /// <returns></returns>
        private FillState GetCurrentFillState(float leftBorder, float rightBorder)
        {
            var currentFillAmount = fillAmount;
            if (currentFillAmount < Mathf.Epsilon) return FillState.Empty;

            if (currentFillAmount >= 1f) return FillState.Full;

            if (currentFillAmount > rightBorder) return FillState.RightSection;

            if (currentFillAmount > leftBorder) return FillState.MidSection;

            return FillState.LeftSection;
        }

        /// <summary>
        /// Get the InterpolationValue, a value important for interpolation of vertex positions, colors, and uv's.
        /// </summary>
        /// <param name="currentFillState">Which section are we currently in</param>
        /// <param name="relativeLeftBorder">Where the left section starts</param>
        /// <param name="relativeRightBorder">Where the right section starts</param>
        /// <returns>This value determines where on a scale from 0 to 1 the fill is in is current section.</returns>
        private float GetInterpolationValue(FillState currentFillState, float relativeLeftBorder, float relativeRightBorder)
        {
            var interpolationValue = 1f;
            if (_fill)
                switch (currentFillState)
                {
                    case FillState.Empty:
                    case FillState.LeftSection:
                        interpolationValue = Mathf.Clamp01(fillAmount / relativeLeftBorder);
                        break;
                    case FillState.MidSection:
                        interpolationValue = Mathf.Clamp01((fillAmount - relativeLeftBorder) /
                                                           (1 - relativeLeftBorder - (1 - relativeRightBorder)));
                        break;
                    case FillState.RightSection:
                    case FillState.Full:
                        interpolationValue =
                                Mathf.Clamp01((fillAmount - relativeRightBorder) / (1 - relativeRightBorder));
                        break;
                    default:
                        // Well, this should never happen
                        Debug.LogError("[ImprovedDistortedImage] Something unexpected happended. Aborting building DistortedImage!");
                        return interpolationValue;
                }

            return interpolationValue;
        }

        /// <summary>
        /// Get the Vertex Colors for a SlicedSprite
        /// </summary>
        /// <returns></returns>
        private Color[] GetVertexColorsForSliced(Color[] cornerColors,
            FillState currentFillState,
            float interpolationValue,
            float relativeLeftBorder,
            float relativeRightBorder,
            float relativeBottomBorder,
            float relativeTopBorder)
        {
            var vertexColors = new Color[16];

            //outer corners
            vertexColors[0] = cornerColors[0];
            vertexColors[4] = cornerColors[1];

            if (_fill && currentFillState == FillState.RightSection)
            {
                var xValue = Mathf.Lerp(relativeRightBorder, 1f, interpolationValue);
                vertexColors[8] = Color.Lerp(cornerColors[1], cornerColors[2], xValue);
                vertexColors[12] = Color.Lerp(cornerColors[0], cornerColors[3], xValue);
            }
            else
            {
                vertexColors[8] = cornerColors[2];
                vertexColors[12] = cornerColors[3];
            }

            //outer boders bottom/top
            if (_fill && currentFillState == FillState.LeftSection)
            {
                vertexColors[3] = Color.Lerp(cornerColors[0], cornerColors[3], relativeLeftBorder * interpolationValue);
                vertexColors[5] = Color.Lerp(cornerColors[1], cornerColors[2], relativeLeftBorder * interpolationValue);
            }
            else
            {
                vertexColors[3] = Color.Lerp(cornerColors[0], cornerColors[3], relativeLeftBorder);
                vertexColors[5] = Color.Lerp(cornerColors[1], cornerColors[2], relativeLeftBorder);
            }

            if (_fill && currentFillState == FillState.MidSection)
            {
                var xValue = Mathf.Lerp(relativeLeftBorder, relativeRightBorder, interpolationValue);
                vertexColors[13] = Color.Lerp(cornerColors[0], cornerColors[3], xValue);
                vertexColors[11] = Color.Lerp(cornerColors[1], cornerColors[2], xValue);
            }
            else
            {
                vertexColors[13] = Color.Lerp(cornerColors[0], cornerColors[3], relativeRightBorder);
                vertexColors[11] = Color.Lerp(cornerColors[1], cornerColors[2], relativeRightBorder);
            }

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
        /// <param name="currentFillState">Which section are we currently in</param>
        /// <param name="interpolationValue">How far are we inside our current section</param>
        /// <returns></returns>
        private Vector2[] GetUvsForSliced(Vector4 outerUv, Vector4 innerUv, FillState currentFillState, float interpolationValue)
        {
            var uvs = new Vector2[16];

            // outer left edge
            uvs[0] = new Vector2(outerUv.x, outerUv.y);
            uvs[1] = new Vector2(outerUv.x, innerUv.y);
            uvs[4] = new Vector2(outerUv.x, outerUv.w);
            uvs[7] = new Vector2(outerUv.x, innerUv.w);

            // inner left edge
            if (_fill && currentFillState == FillState.LeftSection)
            {
                var uvXValue = Mathf.Lerp(outerUv.x, innerUv.x, interpolationValue);
                uvs[2] = new Vector2(uvXValue, innerUv.y);
                uvs[3] = new Vector2(uvXValue, outerUv.y);
                uvs[5] = new Vector2(uvXValue, outerUv.w);
                uvs[6] = new Vector2(uvXValue, innerUv.w);
            }
            else
            {
                uvs[2] = new Vector2(innerUv.x, innerUv.y);
                uvs[3] = new Vector2(innerUv.x, outerUv.y);
                uvs[5] = new Vector2(innerUv.x, outerUv.w);
                uvs[6] = new Vector2(innerUv.x, innerUv.w);
            }

            // inner right edge
            if (_fill && currentFillState == FillState.MidSection)
            {
                var uvXValue = Mathf.Lerp(innerUv.x, innerUv.z, interpolationValue);
                uvs[10] = new Vector2(uvXValue, innerUv.w);
                uvs[11] = new Vector2(uvXValue, outerUv.w);
                uvs[13] = new Vector2(uvXValue, outerUv.y);
                uvs[14] = new Vector2(uvXValue, innerUv.y);
            }
            else
            {
                uvs[10] = new Vector2(innerUv.z, innerUv.w);
                uvs[11] = new Vector2(innerUv.z, outerUv.w);
                uvs[13] = new Vector2(innerUv.z, outerUv.y);
                uvs[14] = new Vector2(innerUv.z, innerUv.y);
            }

            // outer right edge
            if (_fill && currentFillState == FillState.RightSection)
            {
                var uvXValue = Mathf.Lerp(innerUv.z, outerUv.z, interpolationValue);
                uvs[8] = new Vector2(uvXValue, outerUv.w);
                uvs[9] = new Vector2(uvXValue, innerUv.w);
                uvs[12] = new Vector2(uvXValue, outerUv.y);
                uvs[15] = new Vector2(uvXValue, innerUv.y);
            }
            else
            {
                uvs[8] = new Vector2(outerUv.z, outerUv.w);
                uvs[9] = new Vector2(outerUv.z, innerUv.w);
                uvs[12] = new Vector2(outerUv.z, outerUv.y);
                uvs[15] = new Vector2(outerUv.z, innerUv.y);
            }

            return uvs;
        }

        /// <summary>
        /// Gets the Vertices for a SlicedSprite.
        /// </summary>
        /// <param name="cornerPoints">the four Corner-Points (Bottom Left, Top Left, Top Right, Bottom Right)</param>
        /// <returns>The Vertices for a SlicedSprite</returns>
        private Vector3[] GetVerticesForSliced(FillState currentFillState, float interpolationValue)
        {
            var vertices = new Vector3[16];
            var scaledBorder = Sprite.border / multipliedPixelsPerUnit;

            // Calculating the Outer and Inner edges
            var leftOuter = _calculatedLine0;
            var leftInner = leftOuter;
            leftInner.Distance -= scaledBorder.x;

            var topOuter = _calculatedLine1;
            var topInner = topOuter;
            topInner.Distance -= scaledBorder.w;

            var rightOuter = _calculatedLine2;
            var rightInner = rightOuter;
            rightInner.Distance -= scaledBorder.z;

            var bottomOuter = _calculatedLine3;
            var bottomInner = bottomOuter;
            bottomInner.Distance -= scaledBorder.y;

            if (_fill && currentFillState == FillState.LeftSection)
                leftInner.Distance += scaledBorder.x * (1 - interpolationValue);
            else if (_fill && currentFillState == FillState.RightSection)
                rightOuter.Distance                                                -= (1 - interpolationValue) * scaledBorder.z;
            else if (_fill && currentFillState == FillState.MidSection) rightInner =  Line2d.Lerp(leftInner, rightInner, interpolationValue);

            // Calculating the Vertex positions from the generated Edges

            //Positions
            vertices[0] = leftOuter.Collision(bottomOuter).ValueOrDefault(); // BL
            vertices[1] = leftOuter.Collision(bottomInner).ValueOrDefault();
            vertices[2] = leftInner.Collision(bottomInner).ValueOrDefault();
            vertices[3] = leftInner.Collision(bottomOuter).ValueOrDefault();

            vertices[4] = leftOuter.Collision(topOuter).ValueOrDefault(); // TL
            vertices[5] = leftInner.Collision(topOuter).ValueOrDefault();
            vertices[6] = leftInner.Collision(topInner).ValueOrDefault();
            vertices[7] = leftOuter.Collision(topInner).ValueOrDefault();

            vertices[8] = rightOuter.Collision(topOuter).ValueOrDefault(); // TR
            vertices[9] = rightOuter.Collision(topInner).ValueOrDefault();
            vertices[10] = rightInner.Collision(topInner).ValueOrDefault();
            vertices[11] = rightInner.Collision(topOuter).ValueOrDefault();

            vertices[12] = rightOuter.Collision(bottomOuter).ValueOrDefault(); //BR
            vertices[13] = rightInner.Collision(bottomOuter).ValueOrDefault();
            vertices[14] = rightInner.Collision(bottomInner).ValueOrDefault();
            vertices[15] = rightOuter.Collision(bottomInner).ValueOrDefault();

            return vertices;
        }

        /// <summary>
        /// Build the Triangles for a Sliced Image
        /// </summary>
        /// <param name="vertexHelper"></param>
        /// <param name="currentFillState"></param>
        private void AddIndicesForSliced(VertexHelper vertexHelper, FillState currentFillState)
        {
            vertexHelper.AddTriangle(0, 1, 2); //BL
            vertexHelper.AddTriangle(2, 3, 0);

            vertexHelper.AddTriangle(1, 7, 6); //L
            vertexHelper.AddTriangle(6, 2, 1);

            vertexHelper.AddTriangle(4, 5, 6); //TL
            vertexHelper.AddTriangle(6, 7, 4);

            if (_fill && currentFillState < FillState.MidSection) return;

            vertexHelper.AddTriangle(5, 11, 10); //T
            vertexHelper.AddTriangle(10, 6, 5);

            if (fillCenter) // C
            {
                vertexHelper.AddTriangle(2, 6, 10);
                vertexHelper.AddTriangle(10, 14, 2);
            }

            vertexHelper.AddTriangle(2, 14, 13); //B
            vertexHelper.AddTriangle(13, 3, 2);

            if (_fill && currentFillState < FillState.RightSection) return;

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
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint);

            // Since all normals point outwards the distance is positive when outside of the boundaries
            if (Line2d.SignedDistance(localPoint, _calculatedLine0) > 0) return false;

            if (Line2d.SignedDistance(localPoint, _calculatedLine1) > 0) return false;

            if (Line2d.SignedDistance(localPoint, _calculatedLine2) > 0) return false;

            if (Line2d.SignedDistance(localPoint, _calculatedLine3) > 0) return false;

            return true;
        }

        // Get the corner colors
        private Color[] GetCornerColors()
        {
            // If we have a gradient we want different colors, otherwise all has the primary color
            Color color0, color1, color2, color3;
            switch (_colorFillType)
            {
                case ColorGradientType.Solid:

                    color0 = color;
                    color1 = color0;
                    color2 = color0;
                    color3 = color0;
                    break;
                case ColorGradientType.GradientLeftToRight:
                    color0 = color;
                    color1 = color0;
                    color2 = _additionalColor3;
                    color3 = _additionalColor3;
                    break;
                case ColorGradientType.GradientTopToBottom:
                    color0 = color;
                    color1 = _additionalColor1;
                    color2 = _additionalColor1;
                    color3 = color0;
                    break;
                default:
                case ColorGradientType.Vertices:
                    color0 = color;
                    color1 = _additionalColor1;
                    color2 = _additionalColor2;
                    color3 = _additionalColor3;
                    break;
            }

            color0.a *= _opacity;
            color1.a *= _opacity;
            color2.a *= _opacity;
            color3.a *= _opacity;

            return new[] {color0, color1, color2, color3};
        }
    }
}