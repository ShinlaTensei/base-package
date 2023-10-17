#region Header
// Date: 02/08/2023
// Created by: Huynh Phong Tran
// File name: DerivedFlagAttributeDrawer.cs
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base.Helper;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DerivedFlagAttribute))]
public class DerivedFlagAttributeDrawer : PropertyDrawerBase
{
    private Type         m_baseType;
    private List<string> m_derivedNames = new List<string>();
    
    protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
    {
        object               values = m_derivedNames;
        FieldInfo            fieldInfo = ReflectionUtility.GetField(PropertyUtility.GetTargetObjectWithProperty(property), property.name);

        float propertyHeight = AreValuesValid(values, fieldInfo)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();

        return propertyHeight;
    }

    protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        
        DerivedFlagAttribute flagAttribute = attribute as DerivedFlagAttribute;
        if (flagAttribute is null) return;

        m_baseType = flagAttribute.BaseType;

        Type[]   array  = m_baseType.Assembly.GetExportedTypes().Where(type => type.IsSubclassOf(m_baseType)).ToArray();
        object[] values = new object[array.Length + 1];
        values[0] = "<none>";
        m_derivedNames.Add("<none>");
        for (int i = 0; i < array.Length; ++i)
        {
            string name = array[i].Name;
            m_derivedNames.AddIfNotContains(name);
            values[i + 1] = name;
        }
        
        object    target             = PropertyUtility.GetTargetObjectWithProperty(property);
        FieldInfo dropdownField      = ReflectionUtility.GetField(target, property.name);
        object    selectedValue      = dropdownField.GetValue(target);
        int       selectedValueIndex = m_derivedNames.IndexOf((string)selectedValue);

        if (selectedValueIndex == -1)
        {
            selectedValueIndex = 0;
        }

        NaughtyEditorGUI.Dropdown(rect, property.serializedObject, target, dropdownField, label.text, selectedValueIndex
              , values, m_derivedNames.ToArray());
        
        EditorGUI.EndProperty();
    }
    
    private bool AreValuesValid(object values, FieldInfo dropdownField)
    {
        if (values == null || dropdownField == null)
        {
            return false;
        }

        if ((values is IList && dropdownField.FieldType == GetElementType(values)) ||
            (values is IDropdownList))
        {
            return true;
        }

        return false;
    }
    
    private Type GetElementType(object values)
    {
        Type valuesType  = values.GetType();
        Type elementType = ReflectionUtility.GetListElementType(valuesType);

        return elementType;
    }
}