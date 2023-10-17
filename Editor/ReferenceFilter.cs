using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Base.Editor
{
    public class ObjectContextExtend
    {
        [MenuItem("GameObject/Find Reference", false, 100)]
        public static void FindReferencesToAsset()
        {
            var selected = Selection.activeObject;
            if (selected)
                FindReferencesTo(selected);
        }

        private static void FindReferencesTo(Object to)
        {
            var referencedBy = new List<Object>();
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            bool toIsGameObject = to is GameObject;
            Component[] toComponents = toIsGameObject ? ((GameObject)to).GetComponents<Component>() : null;
            string toName = toIsGameObject ? to.name : $"{to.name}.{to.GetType().Name}";

            for (int j = 0; j < allObjects.Length; j++)
            {
                GameObject go = allObjects[j];
                if (go == to)
                {
                    continue;
                } //ignore this object
                
                if (PrefabUtility.GetPrefabAssetType(go) is PrefabAssetType.Model or PrefabAssetType.Regular or PrefabAssetType.Variant)
                {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(go) == to)
                    {
                        Debug.Log($"referenced by {go.name}, {go.GetType()}", go);
                        referencedBy.Add(go);
                    }
                }

                var components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];

                    if (!component) continue;

                    var so = new SerializedObject(component);
                    var sp = so.GetIterator();
                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (sp.objectReferenceValue == to)
                            {
                                Debug.Log($"'{toName}' referenced by '{component.name}' (Component: '{component.GetType().Name}')", component);
                                referencedBy.Add(component.gameObject);
                            }
                            else if (toComponents != null)
                            {
                                bool found = false;
                                foreach (Component toComponent in toComponents)
                                {
                                    if (sp.objectReferenceValue == toComponent)
                                    {
                                        found = true;
                                        referencedBy.Add(component.gameObject);
                                    }
                                }

                                if (found)
                                    Debug.Log($"'{toName}' referenced by '{component.name}' (Component: '{component.GetType().Name}')", component);
                            }
                        }
                    }
                }
            }

            if (referencedBy.Count > 0)
                Selection.objects = referencedBy.ToArray();
            else Debug.Log($"'{toName}': no references in scene");
        }
    }
}