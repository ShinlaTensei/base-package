using System;
using System.Collections.Generic;
using Base.Logging;
using UnityEngine;
using UnityEngine.U2D;

namespace Base
{
    public partial class AddressableManager
    {
        public static class DeviceHDType
        {
            public static bool IsHD { get; set; } = true;
        }

        public struct QueueSpriteAction
        {
            public Action<Sprite> Callback { get; set; }
            public string SpriteName { get; set; }
        }

        Dictionary<string, SpriteAtlas> m_cachedAtlas = new();
        Dictionary<string, Queue<QueueSpriteAction>> m_actionQueue = new();

        static char[] SEPARATORS = {'[', ']'};
        static string SD_SUFFIX = "SD";

        static string RemoveSuffix(string s, string suffix)
        {
            return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
        }

        static string GetAtlasNameWithSuffix(string atlasName)
        {
            if (atlasName.EndsWith(SD_SUFFIX))
            {
                atlasName = RemoveSuffix(atlasName, SD_SUFFIX);
            }

            return DeviceHDType.IsHD ? atlasName : atlasName + SD_SUFFIX;
        }

        public void ClearAtlases()
        {
            foreach (KeyValuePair<string, SpriteAtlas> pair in m_cachedAtlas)
            {
                Resources.UnloadAsset(pair.Value);
            }

            m_cachedAtlas.Clear();
        }

        void InternalLoadSprite(string atlasName)
        {
            LoadAssetAsync<SpriteAtlas>(atlasName, (spriteAtlas) =>
            {
                if (spriteAtlas != null)
                {
                    m_cachedAtlas[atlasName] = spriteAtlas;
                }

                while (m_actionQueue.TryGetValue(atlasName, out Queue<QueueSpriteAction> queues) && queues.Count > 0)
                {
                    QueueSpriteAction queue = queues.Dequeue();
                    queue.Callback?.Invoke(spriteAtlas.GetSprite(queue.SpriteName));
                }
            });
        }

        /// <summary>
        /// Load Sprite and cached sprite atlas for next time
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void LoadSpriteAsync(string key, Action<Sprite> callback)
        {
            string[] keys = !string.IsNullOrEmpty(key) ? key.Split(SEPARATORS) : null;
            if (keys != null && keys.Length > 1)
            {
                string atlas = keys[0];
                string spriteName = keys[1];
                string atlasName = GetAtlasNameWithSuffix(atlas);
                if (m_cachedAtlas.TryGetValue(atlasName, out SpriteAtlas spriteAtlas) && spriteAtlas)
                {
                    Sprite sprite = spriteAtlas.GetSprite(spriteName);
                    callback?.Invoke(sprite);
                }
                else
                {
                    if (!m_actionQueue.TryGetValue(atlasName, out Queue<QueueSpriteAction> queues) || queues == null)
                    {
                        queues = new Queue<QueueSpriteAction>();
                        m_actionQueue[atlasName] = queues;
                    }

                    queues.Enqueue(new QueueSpriteAction {Callback = callback, SpriteName = spriteName});

                    InternalLoadSprite(atlasName);
                }
            }
            else
            {
                PDebug.ErrorFormat("LoadSpriteAsync error unable to get atlas '{0}'", key);
            }
        }
    }
}
