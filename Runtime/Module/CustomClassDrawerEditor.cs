using UnityEditor;
using UnityEngine;
using Base.Module;

namespace Base.Editor
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomClassDrawer), true)]
    public class CustomClassDrawerEditor : PropertyDrawer
    {
        private GUIStyle textStyle = new GUIStyle();
        
        private Color fontColor = new Color(.15f, .15f, .15f, .6f);
        private Color arrowColor = new Color( 0.15f , 0.15f , 0.15f , 0.75f );
        
        const float TitleHeight = 19;
        const float PostTitleSpace = 0;
        const float RightSpace = 1;
        const float ArrowMargin = 5;
        const float isEnabledWidth = 20;
        
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

	
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float space = 0;

            if( property.isExpanded )
            {
                space = 1.2f * EditorGUI.GetPropertyHeight( property ) + TitleHeight;
            }
            else
            {
                space = TitleHeight;
            }

            return space;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SetColor();

            textStyle.normal.textColor = fontColor;
            textStyle.alignment = TextAnchor.MiddleLeft;

            int initialIndent = EditorGUI.indentLevel;
            float initialFieldWidth = EditorGUIUtility.fieldWidth;
            float initialLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.fieldWidth = 60f;
            

            Rect referenceRect = position;
            referenceRect.height = TitleHeight;

            Rect backgroundRect = position;
            backgroundRect.position = referenceRect.position;
            backgroundRect.width -= RightSpace;
            
            GUI.color = new Color(1f, 1f,1f, .5f);
            GUI.Box(backgroundRect, GUIContent.none, EditorStyles.helpBox);
            GUI.color = Color.white;

            Rect titleRect = referenceRect;
            titleRect.width -= isEnabledWidth;
            titleRect.x += 7f;

            property.isExpanded = true;
            
            GUI.Label(titleRect, property.displayName, textStyle);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel = 1;

                SerializedProperty iterator = property.Copy();

                bool enterChildren = true;

                Rect childRect = referenceRect;
                childRect.y += 2 * EditorGUIUtility.singleLineHeight + PostTitleSpace;
                childRect.height = EditorGUIUtility.singleLineHeight;
                childRect.width -= 10;

                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if (SerializedProperty.EqualContents(iterator, property.GetEndProperty())) break;

                    EditorGUI.PropertyField(childRect, iterator);
                    
                    childRect.y += 1.2f * EditorGUI.GetPropertyHeight( iterator , null , false );
                }
            }
            
            EditorGUI.indentLevel = initialIndent;
            EditorGUIUtility.fieldWidth = initialFieldWidth;
            EditorGUIUtility.labelWidth = initialLabelWidth;
            
            EditorGUI.EndProperty();
        }

        private void SetColor()
        {
            fontColor = new Color(.75f, .75f, .75f);
            arrowColor = new Color( 0.15f , 0.15f , 0.15f , 0.75f );
        }
    }
    #endif
}

