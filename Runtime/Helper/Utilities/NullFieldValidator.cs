using System.Collections.Generic;
#if UNITY_EDITOR
using Base.Logging;
using UnityEditor;
#endif // UNITY_EDITOR
using UnityEngine;

namespace Base.Helper
{
    public static class NullFieldValidator
    {
        //<summary> Call this from a monobehavior to validate all SerializedFields. Return true if any field are null </summary>
        public static bool CheckNullAllSerializedFields(this UnityEngine.Object script, string[] ignoreList = null)
        {
#if UNITY_EDITOR
            // alway show warning is really annoy, so save to a list, if list.Count > 0, then print to console
            List<string> warningMessageList = new List<string>();
            // in Unity, to check value of fields of an object in scene, we have to use SerializedObject, not the normal C# reflection FieldInfo
            // https://docs.unity3d.com/ScriptReference/SerializedObject.html
            var serializeObject = new SerializedObject(script);
            var iterator = serializeObject.GetIterator();
            // iterate through all serializedField
            while (iterator.NextVisible(true))
            {
                // check contains in ignoreList
                if (ignoreList != null && ContainInStringList(iterator.name, ignoreList)) continue;
                /// NOTES : if the SerializeProperty.type == "string", then it's isArray also = true
                /// we should ignore this case, it cause "operation is not possible when moved past all properties" exception
                /// https://docs.unity3d.com/ScriptReference/SerializedProperty.html
                const string EXCLUDE_STRING_TYPE = "string";
                if (iterator.type == EXCLUDE_STRING_TYPE)
                {
                    continue;
                }

                // array?
                if (iterator.isArray)
                {
                    var arraySize = iterator.arraySize;
                    var arrayName = iterator.displayName;
                    /// If the SerializeProperty is an Array with size n. Then the next element will the
                    /// SerializeFIeld with name "Size" - which display in inspector just before the array expand.
                    /// Then, their will be the n SerializeProperty corressponding to the array.
                    /// So, call NextVisible here, to skip the "Size" property which we dont need to check
                    iterator.NextVisible(true);
                    /// Loop through n properties in the Array,
                    /// Use NextVisible n times to do this
                    for (int n = 0; n < arraySize; n++)
                    {
                        if (!iterator.NextVisible(true)) break;
                        // a bit tricky here. If we got an UnityEvent in our MonoBehavior, it's callbacks array also get serialized, and if we not config the callback (which is okay, not affect Monobehaivor logi) then the "anyNullField" will also be turned to false. Currently there's no way to detect this case but checking the propertyPath.
                        // The full PropertyPath of an UnityEvent often like this [OureventName].m_PersistentCalls.m_Calls.Array.data[0].m_Target
                        // propertyPath for event with parameter [OurEventName].m_PersistentCalls.m_Calls.Array.data[0].m_Arguments.m_ObjectArgument
                        const string UNITYEVENT_CALLBACK_NAME = ".m_PersistentCalls";
                        if (iterator.propertyPath.Contains(UNITYEVENT_CALLBACK_NAME))
                        {
                            continue;
                        }

                        // "normal" array
                        if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue == null)
                        {
                            warningMessageList.Add($"Array[{arrayName}], null at[{n}], propertyPath [{iterator.propertyPath}]");
                        }
                    }
                }
                // not array, normal field
                else
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue == null)
                    {
                        // a bit tricky here. If we got an UnityEvent in our MonoBehavior, it's callbacks array also get serialized, and if we not config the callback (which is okay, not affect Monobehaivor logi) then the "anyNullField" will also be turned to false. Currently there's no way to detect this case but checking the propertyPath.
                        // The full PropertyPath of an UnityEvent often like this [OureventName].m_PersistentCalls.m_Calls.Array.data[0].m_Target
                        // propertyPath for event with parameter [OurEventName].m_PersistentCalls.m_Calls.Array.data[0].m_Arguments.m_ObjectArgument
                        const string UNITYEVENT_CALLBACK_NAME = ".m_PersistentCalls";
                        if (iterator.propertyPath.Contains(UNITYEVENT_CALLBACK_NAME))
                        {
                            continue;
                        }

                        // normal case
                        warningMessageList.Add($"NullField [{iterator.displayName}], propertyPath[{iterator.propertyPath}]");
                    }
                }
            }

            // check warning list
            if (warningMessageList.Count > 0)
            {
                Debug.LogWarning($"---- [{script.name}], Found [{warningMessageList.Count} null field", script);
                for (int n = 0; n < warningMessageList.Count; n++)
                {
                    Debug.Log(warningMessageList[n], script);
                }
            }

            //Debug.Log($"------- Done Validate [{script.name}]");
            // return result
            return warningMessageList.Count > 0;
#else //UNITY_EDITOR
		Debug.LogWarning("CheckNullAllSerializedFields only work in EDITOR");
		return false;
#endif //UNITY_EDITOR

            /*
            // get the the fields of component
            var allFields = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            // iterate all fields
            for(int n = 0; n < allFields.Length; n++) {
                var field = allFields[n];
                // we will work with public or [SerializedField] fields only
                var serializedAttribute = Attribute.GetCustomAttribute(field, typeof(SerializeField));
                if(field.IsPublic || serializedAttribute != null) {
                    var fieldType = field.FieldType;
                    //var classType = field.GetType();
                    //Log.Info($"Field name [{field.Name}], type [{fieldType}], classType [{classType}], IsClass [{fieldType.IsClass}], IsArray [{fieldType.IsArray}]");
                    if(fieldType.IsValueType) {
                        Log.Warning(false, script, $"Field [{field.Name}] is ValueType, so never NULL, the ValidateNull attribute won't work for it");
                        continue;
                    }
                    // okay, it class type now, check if it's array or not
                    if(fieldType.IsArray) {
                        // cast the value to array
                        // https://stackoverflow.com/questions/3194287/c-sharp-reflection-how-to-get-an-array-values-length?rq=1
                        var array = field.GetValue(script) as Array;
                        for(int m = 0; m < array.Length; m++) {
                            var item = array.GetValue(m);
                            if(item == null) {
                                Log.Warning(false, script, $"Array [{field.Name}, null as index [{m}]]");
                            }
                        }
                    }
                    // not array, simple check null
                    else {
                        var fieldValue = field.GetValue(script);
                        if(fieldValue == null) {
                            Log.Warning(false, script, $"NUll field [{field.Name}]");
                        }
                    }
                }
            }
            */
        }

        public static void ValidatePathStrings(ScriptableObject so)
        {
#if UNITY_EDITOR
            var serializedObject = new SerializedObject(so);
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.type != "string")
                {
                    continue;
                }

                if (!iterator.editable)
                {
                    continue;
                }

                if (iterator.stringValue.Equals(""))
                {
                    Debug.LogError($"Missing path [{iterator.name}]");
                }
            }
#endif
        }

        static bool ContainInStringList(string needToCheck, string[] checkList)
        {
            for (int n = 0; n < checkList.Length; n++)
            {
                if (checkList[n] == needToCheck) return true;
            }

            // not found?
            return false;
        }
    }
}