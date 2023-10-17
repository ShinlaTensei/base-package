#region Header
// Date: 29/05/2023
// Created by: Huynh Phong Tran
// File name: CameraInstance.cs
#endregion

using System;
using Base.Helper;
using Base.Pattern;
using UnityEngine;

namespace Base.Services
{
    public class CameraInstance : BaseMono
    {
        public enum CameraType
        {
            MainCamera, SideCamera
        }
        
        public record CameraKey
        {
            public CameraType   CameraType { get; private set; }
            public string       SceneName { get; private set; }

            public CameraKey(CameraType type, string sceneName)
            {
                CameraType = type;
                SceneName = sceneName;
            }
        }

        [SerializeField] private CameraType m_cameraType = CameraType.SideCamera;

        private Camera m_camera;

        protected override void Start()
        {
            m_camera = GetComponent<Camera>();
            ServiceLocator.SetCamera(this, CacheGameObject.scene.name, m_cameraType);
            
        }

        public Camera GetCam() => m_camera;
    }
}