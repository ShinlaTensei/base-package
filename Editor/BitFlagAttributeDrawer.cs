using System;
using Base.Helper;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BitFlagAttribute))]
public class BitFlagAttributeDrawer : PropertyDrawer
{
    Type     enumType;
    int      enumLength = 0;
    string[] enumNames;
    bool[]   buttonPressed;
    Rect     buttonPos = new Rect();

    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        BitFlagAttribute bitFlag = attribute as BitFlagAttribute;
        if (bitFlag == null) { return; }

        if (enumLength != bitFlag.Length || enumType != bitFlag.Type)
        {
            enumType      = bitFlag.Type;
            enumLength    = bitFlag.Length;
            enumNames     = bitFlag.EnumNames;
            buttonPressed = new bool[enumLength];
        }

        for (int i = 0; i < enumLength; i++) { buttonPressed[i] = false; }

        int itemPerRow = Math.Min(bitFlag.ValuePerRow, enumLength);
        int numberOfRow = enumLength < bitFlag.ValuePerRow ? 1 :
                              enumLength % bitFlag.ValuePerRow == 0 ?
                                  enumLength / bitFlag.ValuePerRow :
                                  enumLength / bitFlag.ValuePerRow + 1;

        float clearButtonWidth = 20f;
        int   buttonsIntValue  = 0;
        float buttonWidth      = (_position.width - EditorGUIUtility.labelWidth - clearButtonWidth) / (itemPerRow != 0 ? itemPerRow : 1);
        float buttonHeight     = _position.height;
        int   nullValue        = bitFlag.HasNull ? (int)Bit.BitValue<long>(0) : 0;

        EditorGUI.LabelField(new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, buttonHeight), _label);
        EditorGUI.BeginChangeCheck();

        float startPosX = _position.x + EditorGUIUtility.labelWidth;
        float nextPosX  = startPosX;
        float nextPosY  = _position.y;
        {
            bool clear = _property.intValue == nullValue;
            buttonPos.Set(nextPosX, nextPosY, clearButtonWidth, buttonHeight);
            clear = GUI.Toggle(buttonPos, clear, "X", "Button");
            if (clear) { _property.intValue = nullValue; }
            nextPosX += clearButtonWidth;
        }

        bool hasValidBit = false;
        int  prevRow     = 0;
        for (int i = 0; i < enumLength; i++)
        {
            int row = i / bitFlag.ValuePerRow;
            int col = i % bitFlag.ValuePerRow;
            if (row != prevRow)
            {
                nextPosX = startPosX;
                prevRow  = row;
            }
            float width = (row == 0 && col == 0) ? buttonWidth - clearButtonWidth : buttonWidth;

            int value = (int)Enum.Parse(enumType, enumNames[i], true);

            // Check if the button is/was pressed
            if (Bit.HasBit(_property.intValue, value)) { buttonPressed[i] = true; }

            buttonPos.Set(nextPosX, nextPosY + row * buttonHeight, width, buttonHeight);
            buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], enumNames[i], "Button");

            if (buttonPressed[i]) { Bit.AddBit(ref buttonsIntValue, value); }

            nextPosX += width;

            if (bitFlag.HasNull && value != 0 && buttonPressed[i] == true || !bitFlag.HasNull && buttonPressed[i] == true)
            {
                hasValidBit = true;
            }
        }

        for (int i = 0; i < numberOfRow; i++)
        {
            GUILayout.Space(20);
        }
        if (hasValidBit && bitFlag.HasNull)
        {
            Bit.ClearBit(ref buttonsIntValue, 0);
        }

        if (EditorGUI.EndChangeCheck())
        {
            _property.intValue = buttonsIntValue;
        }
    }
}
