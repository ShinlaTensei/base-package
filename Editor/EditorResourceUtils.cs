using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


namespace Base.Helper
{
    public static class EditorResourceUtils
    {
        
        [MenuItem("Tools/[Texture] Calculate Optimize Texture Size", false, 13001)]
        [MenuItem("Assets/Utils/Texture/Calculate Optimize Texture Size")]
        public static void CalculateOptimizeTextureSize()
        {
            Object[] objects = Selection.objects;
            for (int i = 0; i < objects.Length; i++)
            {
                Object o = objects[i];
                if (o is Texture obj && obj != null)
                {
                    CalculateOptimizeTextureSize(obj);
                }
                else
                {
                    string   selectedPath = AssetDatabase.GetAssetPath(o);
                    string[] guids        = AssetDatabase.FindAssets("t:Texture", new[] { selectedPath });
                    for (int index = 0; index < guids.Length; index++)
                    {
                        string  guid = guids[index];
                        string  path = AssetDatabase.GUIDToAssetPath(guid);
                        Texture t    = AssetDatabase.LoadAssetAtPath<Texture>(path);
                        if (t != null)
                        {
                            CalculateOptimizeTextureSize(t);
                        }
                    }
                }
            }
        }
        public static Vector2 GetOriginalTextureSize(TextureImporter importer)
        {
            Vector2 originalSize = new Vector2();
            int[] args = new int[2];
            importer.GetSourceTextureWidthAndHeight(out args[0], out args[1]);
            originalSize.x = args[0];
            originalSize.y = args[1];

            return originalSize;
        }

        static void CalculateOptimizeTextureSize(Texture obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            Vector2 original = GetOriginalTextureSize(importer);
            int originalWidth = (int)original.x;
            int originalHeight = (int)original.y;
            int originalMaxSize = Math.Max(originalWidth, originalHeight);

            int maxSize = Math.Max(obj.width, obj.height);
            int width = obj.width;
            int height = obj.height;

            float scale = (float)originalMaxSize / maxSize;
            bool optimizeWidth = width % 4 == 0;
            bool optimizeHeight = height % 4 == 0;

            Vector2 minShouldBe = new Vector2(originalWidth, originalHeight);
            Vector2 maxShouldBe = new Vector2(originalWidth, originalHeight);

            if (!optimizeWidth)
            {
                int down = Mathf.FloorToInt(width / 4.0f) * 4;
                int up = Mathf.FloorToInt(width / 4.0f + 0.5f) * 4;
                int sizeDown = Mathf.FloorToInt(scale * down);
                int sizeUp = Mathf.FloorToInt(scale * up + 0.5f);

                minShouldBe.x = sizeDown;
                maxShouldBe.x = sizeUp;
            }

            if (!optimizeHeight)
            {
                int down = Mathf.FloorToInt(height / 4.0f) * 4;
                int up = Mathf.FloorToInt(height / 4.0f + 0.5f) * 4;
                int sizeDown = Mathf.FloorToInt(scale * down);
                int sizeUp = Mathf.FloorToInt(scale * up + 0.5f);
                minShouldBe.y = sizeDown;
                maxShouldBe.y = sizeUp;
            }

            if (!optimizeWidth || !optimizeHeight)
            {
                string info = $"[Optimize]" +
                              $"O=[{originalWidth}:{originalHeight}] T=[{width}:{height}] " +
                              $"--> " +
                              $"Min<b><color=green>[{(int)minShouldBe.x}:{(int)minShouldBe.y}]</color></b> " +
                              $"Max<b><color=green>[{(int)maxShouldBe.x}:{(int)maxShouldBe.y}]</color></b> " +
                              $"--> '{path}'";
                Debug.LogFormat(obj, info);
            }
        }
    }
}