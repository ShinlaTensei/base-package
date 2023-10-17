using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace Base
{
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] private bool showFps;
        [SerializeField] private bool showMemory;
        [SerializeField] private Texture2D background;

        public bool Debug
        {
            get => debug;
            set => debug = value;
        }
        
        public bool ShowFps
        {
            get => showFps;
            set => showFps = value;
        }

        public bool ShowMemory
        {
            get => showMemory;
            set => showMemory = value;
        }

        private static Process _currentProcess;
        private GUIStyle _style;
        private float _deltaTime = 0.0f;
        private int _textSize;
        private int _screenWidth;
        private int _screenHeight;

#if CHEAT_ENABLE
        private void Start()
        {
            _currentProcess = Process.GetCurrentProcess();

            _screenWidth = Screen.width; _screenHeight = Screen.height;
            
            _textSize = _screenHeight * 2 / 100;
            
            _style = new GUIStyle();
            _style.alignment = TextAnchor.UpperLeft;
            _style.fontSize = _textSize;
            _style.normal.textColor = Color.white;
            _style.normal.background = background;
        }

        void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!debug) return;

            float posY = 0;
            if (showFps)
            {
                Rect rect = new Rect(0, posY, _screenWidth, _textSize);
                posY += rect.height;
                float msec = _deltaTime * 1000.0f;
                float fps = 1.0f / _deltaTime;
                string text = string.Format("FPS: {0:0.0} ms ({1:0.} fps)", msec, fps);
                GUI.Label(rect, text, _style);
            }

            if (showMemory)
            {
                Rect memoryRect = new Rect(0, posY, _screenWidth, _screenHeight * 2 / 50f);
                posY += memoryRect.height;
                string text = GetMemorySize();
                GUI.Label(memoryRect, text, _style);
            }
        }

        private string GetMemorySize()
        {
            string text = "RAM: " +
                          $"T:{SystemInfo.systemMemorySize}MB - U:{_currentProcess.WorkingSet64 / (1024 * 1024)}MB - " +
                          $"R:{Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)}MB - " +
                          $"A:{Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)}MB - " +
                          $"F:{Profiler.GetTotalUnusedReservedMemoryLong() / (1024 * 1024)}MB \n          " +
                          $"G:{Profiler.GetAllocatedMemoryForGraphicsDriver() / (1024 * 1024)}MB - " +
                          " Mono: " +
                          $"H:{Profiler.GetMonoHeapSizeLong() / (1024 * 1024)}MB - " +
                          $"U:{Profiler.GetMonoUsedSizeLong() / (1024 * 1024)}MB";

            return text;
        }
#endif
    }
}
