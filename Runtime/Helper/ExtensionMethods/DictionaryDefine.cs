#region Header
// Date: 22/07/2023
// Created by: Huynh Phong Tran
// File name: DictionaryDefine.cs
#endregion

using System;
using UnityEngine;
using UnityEngine.U2D;

namespace Base.Helper
{
    [Serializable]
    public class BlueprintMapper : KnDictionary<string, bool>
    {
    }
    [Serializable]
    public class PrefabsMapper : KnDictionary<string, GameObject>
    {
    }

    [Serializable]
    public class SpriteMapper : KnDictionary<string, Sprite>
    {
    }

    [Serializable]
    public class PathMapper : KnDictionary<string, string>
    {
    }

    [Serializable]
    public class BoolMapper : KnDictionary<string, bool>
    {
    }
    
    [Serializable]
    public class IntMapper : KnDictionary<string, int>
    {
    }

    [Serializable]
    public class StringMapper : KnDictionary<string, string>
    {
    }

    [Serializable]
    public class AtlasMapper : KnDictionary<string, SpriteAtlas>
    {
    }
}