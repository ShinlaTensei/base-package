#region Header
// Date: 12/10/2023
// Created by: Huynh Phong Tran
// File name: DistortedImageEditor.cs
#endregion

using Base.Helper;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(DistortedImage))]
    public class DistortedImageEditor : UnityEditor.Editor
    {
        private bool _editInSceneView = false;

        public override void OnInspectorGUI()
        {
            if (!(target is DistortedImage distortedImage))
            {
                GUILayout.Label("Wrong target type!");
                return;
            }

            serializedObject.Update();

            SerializedProperty spriteProperty = serializedObject.FindProperty("m_Sprite");
            EditorGUILayout.PropertyField(spriteProperty);

            SerializedProperty materialProperty = serializedObject.FindProperty("m_Material");
            EditorGUILayout.PropertyField(materialProperty);

            SerializedProperty hideCenterProperty = serializedObject.FindProperty("m_FillCenter");
            EditorGUILayout.PropertyField(hideCenterProperty);

            SerializedProperty maskableProperty = serializedObject.FindProperty("m_Maskable");
            EditorGUILayout.PropertyField(maskableProperty);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Color", EditorStyles.boldLabel);
                SerializedProperty colorFillTypeProperty = serializedObject.FindProperty("_colorFillType");
                EditorGUILayout.PropertyField(colorFillTypeProperty);

                switch (colorFillTypeProperty.enumValueIndex)
                {
                    case 0: // solid
                        DrawColorPropertiesSolid();
                        break;
                    case 1: // horizontal
                        DrawColorPropertiesHorizontal();
                        break;
                    case 2: // vertical
                        DrawColorPropertiesVertical();
                        break;
                    case 3: // Vertices
                        DrawColorPropertiesVertex();
                        break;
                }

                SerializedProperty opacityProperty = serializedObject.FindProperty("_opacity");
                EditorGUILayout.PropertyField(opacityProperty);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Fill", EditorStyles.boldLabel);
                SerializedProperty fillProperty = serializedObject.FindProperty("_fill");
                EditorGUILayout.PropertyField(fillProperty, new GUIContent("Has Fill"));

                if (fillProperty.boolValue)
                {
                    SerializedProperty fillAmountProperty = serializedObject.FindProperty("m_FillAmount");
                    EditorGUILayout.PropertyField(fillAmountProperty);

                    SerializedProperty fillDirectionProperty = serializedObject.FindProperty("_fillDirection");
                    EditorGUILayout.PropertyField(fillDirectionProperty);
                    if (fillDirectionProperty.enumValueIndex != 0 && spriteProperty.objectReferenceValue is Sprite &&
                        ((Sprite) spriteProperty.objectReferenceValue).border.sqrMagnitude > 0)
                    {
                        EditorGUILayout.HelpBox(
                            "Warning: Fill Direction only implemented for non-sliced sprites! The current sprite is sliced, direction will be left-to-right!",
                            MessageType.Warning);
                    }
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Offsets:", EditorStyles.boldLabel);
                _editInSceneView = EditorGUILayout.Toggle("Edit in SceneView", _editInSceneView);

                SerializedProperty
                    offset0Property = serializedObject.FindProperty("_offsetBottomLeft"),
                    offset1Property = serializedObject.FindProperty("_offsetTopLeft"),
                    offset2Property = serializedObject.FindProperty("_offsetTopRight"),
                    offset3Property = serializedObject.FindProperty("_offsetBottomRight");
                GUILayout.BeginHorizontal();
                GUILayout.Space(81);
                GUILayout.Label("Left", GUILayout.Width(60));
                GUILayout.FlexibleSpace();
                GUILayout.Space(21);
                GUILayout.Label("Right", GUILayout.Width(60));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Top", GUILayout.Width(60));
                EditorGUILayout.PropertyField(offset1Property, new GUIContent());
                EditorGUILayout.PropertyField(offset2Property, new GUIContent());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Bottom", GUILayout.Width(60));
                EditorGUILayout.PropertyField(offset0Property, new GUIContent());
                EditorGUILayout.PropertyField(offset3Property, new GUIContent());
                GUILayout.EndHorizontal();
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Margins:", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftFrame"), new GUIContent("Left"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_topFrame"), new GUIContent("Top"));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightFrame"), new GUIContent("Right"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_bottomFrame"), new GUIContent("Bottom"));
                GUILayout.EndHorizontal();
            }

            if (HasSlicedSprite(serializedObject, out Sprite sprite))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("UnitsPerPixel:", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PixelsPerUnitMultiplier"));
                    GUI.enabled = false;
                    EditorGUILayout.FloatField("Sprite Pixels per Unit", sprite.pixelsPerUnit);
                    EditorGUILayout.FloatField("Total Pixels per Unit", distortedImage.pixelsPerUnit * distortedImage.pixelsPerUnitMultiplier);
                    GUI.enabled = true;
                }
            }

            SerializedProperty raycastProperty = serializedObject.FindProperty("m_RaycastTarget");
            EditorGUILayout.PropertyField(raycastProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private static bool HasSlicedSprite(SerializedObject so, out Sprite sprite)
        {
            sprite = so.FindProperty("m_Sprite").objectReferenceValue as Sprite;
            return sprite != null && sprite.border.sqrMagnitude > 0f;
        }

        private void DrawColorPropertiesSolid()
        {
            SerializedProperty colorProperty = serializedObject.FindProperty("m_Color");
            EditorGUILayout.PropertyField(colorProperty, new GUIContent());
        }

        private void DrawColorPropertiesVertical()
        {
            SerializedProperty topColorPorperty = serializedObject.FindProperty("_additionalColor1");
            SerializedProperty bottomColorProperty = serializedObject.FindProperty("m_Color");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Top", GUILayout.Width(60));
            EditorGUILayout.PropertyField(topColorPorperty, new GUIContent());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bottom", GUILayout.Width(60));
            EditorGUILayout.PropertyField(bottomColorProperty, new GUIContent());
            GUILayout.EndHorizontal();
        }

        private void DrawColorPropertiesHorizontal()
        {
            SerializedProperty leftColorProperty = serializedObject.FindProperty("m_Color");
            SerializedProperty rightColorProperty = serializedObject.FindProperty("_additionalColor3");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Left", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Right", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(leftColorProperty, new GUIContent());
            EditorGUILayout.PropertyField(rightColorProperty, new GUIContent());
            GUILayout.EndHorizontal();
        }

        private void DrawColorPropertiesVertex()
        {
            SerializedProperty colorProperty = serializedObject.FindProperty("m_Color");
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            GUILayout.Label("Left", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Right", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Top", GUILayout.Width(60));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_additionalColor1"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_additionalColor2"), new GUIContent());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bottom", GUILayout.Width(60));
            EditorGUILayout.PropertyField(colorProperty, new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_additionalColor3"), new GUIContent());
            GUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            Object o = target;
            SerializedObject so = new SerializedObject(o);
            ShowOutlineEdit(so);
        }

        private void ShowOutlineEdit(SerializedObject so)
        {
            so.Update();
            SerializedProperty[] offsetProperties = new SerializedProperty[4];
            offsetProperties[0] = so.FindProperty("_offsetBottomLeft");
            offsetProperties[1] = so.FindProperty("_offsetTopLeft");
            offsetProperties[2] = so.FindProperty("_offsetTopRight");
            offsetProperties[3] = so.FindProperty("_offsetBottomRight");

            if (!(target is Graphic))
            {
                Debug.LogError(
                    "[ImprovedDistortedImageEditor] The DistortedImageEditor inspects a non Graphic. Something has gone terribly wrong in Unity.");
                return;
            }

            RectTransform targetRectTransform = ((Graphic) target).rectTransform;
            Rect rect = targetRectTransform.rect;
            Vector4 rectDimensions = new Vector4
            (
                rect.x,
                rect.y,
                rect.x + rect.width,
                rect.y + rect.height
            );
            Vector3[] positions = new Vector3[4];

            // Get Local Positions
            positions[0] = CornerToGlobalPosition(offsetProperties[0].vector2Value,
                new Vector3(rectDimensions.x, rectDimensions.y), targetRectTransform);
            positions[1] = CornerToGlobalPosition(offsetProperties[1].vector2Value,
                new Vector3(rectDimensions.x, rectDimensions.w), targetRectTransform);
            positions[2] = CornerToGlobalPosition(offsetProperties[2].vector2Value,
                new Vector3(rectDimensions.z, rectDimensions.w), targetRectTransform);
            positions[3] = CornerToGlobalPosition(offsetProperties[3].vector2Value,
                new Vector3(rectDimensions.z, rectDimensions.y), targetRectTransform);

            Color colorBefore = Handles.color;
            Handles.color = Color.white;

            // Draw Outside Edges
            Handles.DrawLine(positions[0], positions[1]);
            Handles.DrawLine(positions[1], positions[2]);
            Handles.DrawLine(positions[2], positions[3]);
            Handles.DrawLine(positions[3], positions[0]);

            if (_editInSceneView)
            {
                Handles.color = Color.yellow;

                Quaternion rotation = targetRectTransform.rotation;

                // Draw Corner Handles
#if UNITY_2022_1_OR_NEWER
                positions[0] = Handles.FreeMoveHandle(positions[0],
                    HandleUtility.GetHandleSize(positions[0]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[1] = Handles.FreeMoveHandle(positions[1],
                    HandleUtility.GetHandleSize(positions[1]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[2] = Handles.FreeMoveHandle(positions[2],
                    HandleUtility.GetHandleSize(positions[2]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[3] = Handles.FreeMoveHandle(positions[3],
                    HandleUtility.GetHandleSize(positions[3]) * .1f, Vector3.one, Handles.CircleHandleCap);
#else
                positions[0] = Handles.FreeMoveHandle(positions[0], rotation,
                    HandleUtility.GetHandleSize(positions[0]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[1] = Handles.FreeMoveHandle(positions[1], rotation,
                    HandleUtility.GetHandleSize(positions[1]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[2] = Handles.FreeMoveHandle(positions[2], rotation,
                    HandleUtility.GetHandleSize(positions[2]) * .1f, Vector3.one, Handles.CircleHandleCap);
                positions[3] = Handles.FreeMoveHandle(positions[3], rotation,
                    HandleUtility.GetHandleSize(positions[3]) * .1f, Vector3.one, Handles.CircleHandleCap);
#endif
               

                // Get Global Positions
                offsetProperties[0].vector2Value = GlobalPositionToCorner(positions[0],
                    new Vector3(rectDimensions.x, rectDimensions.y), targetRectTransform);
                offsetProperties[1].vector2Value = GlobalPositionToCorner(positions[1],
                    new Vector3(rectDimensions.x, rectDimensions.w), targetRectTransform);
                offsetProperties[2].vector2Value = GlobalPositionToCorner(positions[2],
                    new Vector3(rectDimensions.z, rectDimensions.w), targetRectTransform);
                offsetProperties[3].vector2Value = GlobalPositionToCorner(positions[3],
                    new Vector3(rectDimensions.z, rectDimensions.y), targetRectTransform);

                Handles.color = colorBefore;
            }

            so.ApplyModifiedProperties();
        }

        private Vector3 CornerToGlobalPosition(Vector3 position, Vector3 relativePosition, Transform parentTransform)
        {
            position += relativePosition;
            position = parentTransform.TransformPoint(position);
            return position;
        }

        private Vector3 GlobalPositionToCorner(Vector3 position, Vector3 relativePosition, Transform parentTransform)
        {
            position = parentTransform.InverseTransformPoint(position);
            position -= relativePosition;
            return position;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            DistortedImage image = target as DistortedImage;
            if (image == null)
            {
                return;
            }

            Sprite sf = image.sprite;
            if (sf == null)
            {
                return;
            }

            SpriteUtils.DrawSprite(sf, rect, image.canvasRenderer.GetColor());
        }
    }
}