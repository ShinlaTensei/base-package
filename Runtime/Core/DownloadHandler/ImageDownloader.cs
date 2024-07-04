using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using Base.Helper;
using Base.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Application = UnityEngine.Application;

namespace Base.Core
{
    public class ImageDownloader : IDisposable
    {
        private readonly string SAVE_PATH = Application.persistentDataPath + Path.DirectorySeparatorChar + "textures" + Path.DirectorySeparatorChar;

        private string m_url;
        private string m_hash;
        private Image m_image;
        private SpriteRenderer m_spriteRenderer;
        private CancellationToken m_cancellationToken;

        private Action<string> OnError;
        private Action<Sprite> OnCompleted;

        public static ImageDownloader Create() => new ImageDownloader();

        public ImageDownloader WithError(Action<string> action)
        {
            OnError = action;
            return this;
        }
        public ImageDownloader WithCompleted(Action<Sprite> action)
        {
            OnCompleted = action;
            return this;
        }
        public ImageDownloader Into(Image image)
        {
            m_image = image;
            return this;
        }
        
        public ImageDownloader Into(SpriteRenderer renderer)
        {
            m_spriteRenderer = renderer;
            return this;
        }

        public ImageDownloader AttachToken(CancellationToken token)
        {
            m_cancellationToken = token;
            return this;
        }

        public ImageDownloader Load(string url)
        {
            m_url = url;
            m_hash = CreateMD5(url);
            return this;
        }

        public void Start()
        {
            if (m_url == null)
            {
                Error("Url has not been set. Use Load function to set image url");
                return;
            }

            try
            {
                Uri uri = new Uri(m_url);
                m_url = uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                Error(e.Message);
                return;
            }
            
            PDebug.Info("[ImageDownloader] Start loading image from: " + m_url);

            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
            }

            if (File.Exists(SAVE_PATH + m_hash))
            {
                LoadTextureToSprite();
            }
            else
            {
                TaskRunner.Start(DownloadImage().AsTask(), onError: Error);
            }
        }

        private void Error(string message)
        {
            PDebug.Error(message);
            
            OnError?.Invoke(message);
        }

        private void Error(Exception exception)
        {
            Error(exception.Message);
        }

        private string CreateMD5(string input)
        {
            return Encryption.ToMD5(input);
        }

        private async UniTask DownloadImage()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(m_url);

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            await UniTask.WaitUntil(() => operation.isDone || Application.internetReachability == NetworkReachability.NotReachable
                , cancellationToken: m_cancellationToken);
            
            if (request.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            {
                Error("Error while downloading: " + request.error);
                await UniTask.Yield();
            }
            
            if (request.responseCode == (long) HttpStatusCode.OK)
            {
                if (!File.Exists(SAVE_PATH + m_hash))
                {
                    await File.WriteAllBytesAsync(SAVE_PATH + m_hash, request.downloadHandler.data, m_cancellationToken);
                }
            }
           
            request.Dispose();
            request = null;
            
            LoadTextureToSprite();
        }

        private void LoadTextureToSprite(Texture2D texture = null)
        {
            if (!File.Exists(SAVE_PATH + m_hash))
            {
                Error("Load image file failed! ");
                return;
            }

            if (texture == null)
            {
                byte[] fileData = File.ReadAllBytes(SAVE_PATH + m_hash);
                texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (m_image != null)
            {
                m_image.sprite = sprite;
            }

            if (m_spriteRenderer != null)
            {
                m_spriteRenderer.sprite = sprite;
            }
            
            OnCompleted?.Invoke(sprite);
            
            Dispose();
        }

        public void Dispose()
        {
            m_image = null;
            m_spriteRenderer = null;
            OnCompleted = null;
            OnError = null;
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}