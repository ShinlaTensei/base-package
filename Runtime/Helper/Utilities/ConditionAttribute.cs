using System;
using Base.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionAttribute : PropertyAttribute
    {
        public string ConditionalSourceField = "";
        public string ConditionalSourceField2 = "";
        public string[] ConditionalSourceFields = new string[] { };
        public bool[] ConditionalSourceFieldInverseBools = new bool[] { };
        public bool HideInInspector = false;
        public bool Inverse = false;
        public bool UseOrLogic = false;

        public bool InverseCondition1 = false;
        public bool InverseCondition2 = false;

        // Use this for initialization
        public ConditionAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = false;
            Inverse = false;
        }

        public ConditionAttribute(string conditionalSourceField, bool hideInInspector)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            Inverse = false;
        }

        public ConditionAttribute(string conditionalSourceField, bool hideInInspector, bool inverse)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }

        public ConditionAttribute(bool hideInInspector = false)
        {
            ConditionalSourceField = "";
            HideInInspector = hideInInspector;
            Inverse = false;
        }

        public ConditionAttribute(string[] conditionalSourceFields, bool[] conditionalSourceFieldInverseBools, bool hideInInspector, bool inverse)
        {
            ConditionalSourceFields = conditionalSourceFields;
            ConditionalSourceFieldInverseBools = conditionalSourceFieldInverseBools;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }

        public ConditionAttribute(string[] conditionalSourceFields, bool hideInInspector, bool inverse)
        {
            ConditionalSourceFields = conditionalSourceFields;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionAttribute))]
    public class ConditionalAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionAttribute condHAtt   = (ConditionAttribute) attribute;
            bool               enabled    = GetAttributeResult(condHAtt, property);
            bool               wasEnabled = GUI.enabled;

            GUI.enabled = enabled;
            if (!condHAtt.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = wasEnabled;
        }

        bool GetAttributeResult(ConditionAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = (condHAtt.UseOrLogic) ? false : true;

            //Handle primary property
            SerializedProperty sourcePropertyValue = null;
            //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays
            if (!property.isArray)
            {
                string propertyPath  = property.propertyPath;                                                //returns the property path of the property we want to apply the attribute to
                string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
                sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                //original implementation (doens't work with nested serializedObjects)
                if (sourcePropertyValue == null)
                {
                    sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);
                }
            }
            else
            {
                //original implementation (doens't work with nested serializedObjects)
                sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);
            }


            if (sourcePropertyValue != null)
            {
                enabled = CheckPropertyType(sourcePropertyValue);
                if (condHAtt.InverseCondition1) enabled = !enabled;
            }
            else
            {
                //Debug.LogWarning("Attempting to use a ConditionAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }


            //handle secondary property
            SerializedProperty sourcePropertyValue2 = null;
            if (!property.isArray)
            {
                string propertyPath  = property.propertyPath;                                                 //returns the property path of the property we want to apply the attribute to
                string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField2); //changes the path to the conditionalsource property path
                sourcePropertyValue2 = property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                //original implementation (doens't work with nested serializedObjects)
                if (sourcePropertyValue2 == null)
                {
                    sourcePropertyValue2 = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField2);
                }
            }
            else
            {
                // original implementation(doens't work with nested serializedObjects) 
                sourcePropertyValue2 = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField2);
            }

            //Combine the results
            if (sourcePropertyValue2 != null)
            {
                bool prop2Enabled = CheckPropertyType(sourcePropertyValue2);

                if (condHAtt.InverseCondition2) { prop2Enabled = !prop2Enabled; }
                if (condHAtt.UseOrLogic) { enabled = enabled || prop2Enabled; }
                else { enabled                     = enabled && prop2Enabled; }
            }
            else
            {
                //Debug.LogWarning("Attempting to use a ConditionAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            //Handle the unlimited property array
            string[] conditionalSourceFieldArray        = condHAtt.ConditionalSourceFields;
            bool[]   conditionalSourceFieldInverseArray = condHAtt.ConditionalSourceFieldInverseBools;
            for (int index = 0; index < conditionalSourceFieldArray.Length; ++index)
            {
                SerializedProperty sourcePropertyValueFromArray = null;
                if (!property.isArray)
                {
                    string propertyPath  = property.propertyPath;                                                   //returns the property path of the property we want to apply the attribute to
                    string conditionPath = propertyPath.Replace(property.name, conditionalSourceFieldArray[index]); //changes the path to the conditionalsource property path
                    sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionPath);

                    //if the find failed->fall back to the old system
                    if (sourcePropertyValueFromArray == null)
                    {
                        //original implementation (doens't work with nested serializedObjects)
                        sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionalSourceFieldArray[index]);
                    }
                }
                else
                {
                    // original implementation(doens't work with nested serializedObjects) 
                    sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionalSourceFieldArray[index]);
                }

                //Combine the results
                if (sourcePropertyValueFromArray != null)
                {
                    bool propertyEnabled                                                                                                       = CheckPropertyType(sourcePropertyValueFromArray);
                    if (conditionalSourceFieldInverseArray.Length >= (index + 1) && conditionalSourceFieldInverseArray[index]) propertyEnabled = !propertyEnabled;

                    if (condHAtt.UseOrLogic)
                        enabled = enabled || propertyEnabled;
                    else
                        enabled = enabled && propertyEnabled;
                }
                else
                {
                    //Debug.LogWarning("Attempting to use a ConditionAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
                }
            }


            //wrap it all up
            if (condHAtt.Inverse) enabled = !enabled;

            return enabled;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionAttribute condHAtt = (ConditionAttribute) attribute;
            bool               enabled  = GetAttributeResult(condHAtt, property);

            if (!condHAtt.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                //The property is not being drawn
                //We want to undo the spacing added before and after the property
                return -EditorGUIUtility.standardVerticalSpacing;
                //return 0.0f;
            }
        }
        bool CheckPropertyType(SerializedProperty sourcePropertyValue)
        {
            //Note: add others for custom handling if desired
            switch (sourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return sourcePropertyValue.boolValue;
                case SerializedPropertyType.ObjectReference:
                    return sourcePropertyValue.objectReferenceValue != null;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
                    return true;
            }
        }
    }

#endif