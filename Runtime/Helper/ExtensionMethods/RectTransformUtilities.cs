using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    public static class RectTransformUtils
    {
        public static float GetCanvasScaleOnWidth(float crrWidth, Canvas canvas)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            return crrWidth / scaler.referenceResolution.x;
        }
        
        public static float GetCanvasScaleOnHeight(float crrHeight, Canvas canvas)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            return crrHeight / scaler.referenceResolution.y;
        }
    }
}

