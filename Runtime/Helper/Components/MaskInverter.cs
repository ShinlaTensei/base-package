#region Header
// Date: 10/08/2023
// Created by: Huynh Phong Tran
// File name: MaskInverter.cs
#endregion

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Base.Helper
{
    [AddComponentMenu("Base/UI/Mask Inverter")]
    public sealed class MaskInverter : MonoBehaviour, IMaterialModifier
    {
        private static readonly int _stencilComp = Shader.PropertyToID("_StencilComp");
        
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            var resultMaterial = new Material(baseMaterial);
            resultMaterial.SetFloat(_stencilComp, Convert.ToSingle(CompareFunction.NotEqual));
            return resultMaterial;
        }
    }
}