#region Header
// Date: 04/01/2024
// Created by: Huynh Phong Tran
// File name: ListItemSelectorAttributeDrawer.cs
#endregion

using UnityEngine;
using System;
using Base.CustomAttribute;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using Sirenix.Utilities;

[DrawerPriority(0.01, 0, 0)]
public class ListItemSelectorAttributeDrawer : OdinAttributeDrawer<ListItemSelectorAttribute>
{
        private static Color selectedColor = new Color(0.301f, 0.563f, 1f, 0.497f);
    private bool isListElement;
    private InspectorProperty baseMemberProperty;
    private PropertyContext<InspectorProperty> globalSelectedProperty;
    private InspectorProperty selectedProperty;
    private Action<object, int> selectedIndexSetter;

    protected override void Initialize()
    {
        this.isListElement = this.Property.Parent != null && this.Property.Parent.ChildResolver is IOrderedCollectionResolver;
        var isList = !this.isListElement;
        var listProperty = isList ? this.Property : this.Property.Parent;
        this.baseMemberProperty = listProperty.FindParent(x => x.Info.PropertyType == PropertyType.Value, true);
        this.globalSelectedProperty = this.baseMemberProperty.Context.GetGlobal("selectedIndex" + this.baseMemberProperty.GetHashCode(), (InspectorProperty)null);

        if (isList)
        {
            var parentType = this.baseMemberProperty.ParentValues[0].GetType();
            this.selectedIndexSetter = EmitUtilities.CreateWeakInstanceMethodCaller<int>(parentType.GetMethod(this.Attribute.SetSelectedMethod, Flags.AllMembers));
        }
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var t = Event.current.type;

        if (this.isListElement)
        {
            if (t == EventType.Layout)
            {
                this.CallNextDrawer(label);
            }
            else
            {
                var rect = GUIHelper.GetCurrentLayoutRect();
                var isSelected = this.globalSelectedProperty.Value == this.Property;

                if (t == EventType.Repaint && isSelected)
                {
                    EditorGUI.DrawRect(rect, selectedColor);
                }
                else if (t == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    this.globalSelectedProperty.Value = this.Property;
                }

                this.CallNextDrawer(label);

            }
        }
        else
        {
            this.CallNextDrawer(label);

            if (Event.current.type != EventType.Layout)
            {
                var sel = this.globalSelectedProperty.Value;

                // Select
                if (sel != null && sel != this.selectedProperty)
                {
                    this.selectedProperty = sel;
                    this.Select(this.selectedProperty.Index);
                }
                // Deselect when destroyed
                else if (this.selectedProperty != null && this.selectedProperty.Index < this.Property.Children.Count && this.selectedProperty != this.Property.Children[this.selectedProperty.Index])
                {
                    var index = -1;
                    this.Select(index);
                    this.selectedProperty = null;
                    this.globalSelectedProperty.Value = null;
                }
            }
        }
    }

    private void Select(int index)
    {
        GUIHelper.RequestRepaint();
        this.Property.Tree.DelayAction(() =>
        {
            for (int i = 0; i < this.baseMemberProperty.ParentValues.Count; i++)
            {
                this.selectedIndexSetter(this.baseMemberProperty.ParentValues[i], index);
            }
        });
    }
}