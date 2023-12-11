using UnityEngine;
using UnityEditor;
using Base.Helper;

namespace Base.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(GradientImage))]
    public class GradientImageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {

            if (!(target is GradientImage gradientImage))
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"), new GUIContent());
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_additionalColor3"), new GUIContent());
                GUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_opacity"));
            }

            if (HasSlicedSprite(serializedObject, out Sprite sprite))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("UnitsPerPixel:",EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PixelsPerUnitMultiplier"));
                    GUI.enabled = false;
                    EditorGUILayout.FloatField("Sprite Pixels per Unit", sprite.pixelsPerUnit);
                    EditorGUILayout.FloatField("Total Pixels per Unit", gradientImage.pixelsPerUnit * gradientImage.pixelsPerUnitMultiplier);
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

        public override bool HasPreviewGUI() { return true; }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            GradientImage image = target as GradientImage;
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
