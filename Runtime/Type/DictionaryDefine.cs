#region Header
// Date: 22/07/2023
// Created by: Huynh Phong Tran
// File name: DictionaryDefine.cs
#endregion

using System;
using UnityEngine;
using UnityEngine.U2D;

namespace Base.CustomType
{
    [Serializable]
    public class BlueprintMapper : SerializedDictionary<string, bool>
    {
    }
    [Serializable]
    public class PrefabsMapper : SerializedDictionary<string, GameObject>
    {
    }

    [Serializable]
    public class SpriteMapper : SerializedDictionary<string, Sprite>
    {
    }

    [Serializable]
    public class PathMapper : SerializedDictionary<string, string>
    {
    }

    [Serializable]
    public class BoolMapper : SerializedDictionary<string, bool>
    {
    }
    
    [Serializable]
    public class IntMapper : SerializedDictionary<string, int>
    {
    }

    [Serializable]
    public class StringMapper : SerializedDictionary<string, string>
    {
    }

    [Serializable]
    public class AtlasMapper : SerializedDictionary<string, SpriteAtlas>
    {
    }
}