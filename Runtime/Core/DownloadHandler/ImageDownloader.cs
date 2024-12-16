using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using DTT.Utils.Exceptions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Application = UnityEngine.Application;

namespace Base.Core
{
    public class ImageDownloader : IDisposable
    {
        private static string SAVE_PATH = Application.persistentDataPath + Path.DirectorySeparatorChar + "textures" + Path.DirectorySeparatorChar;

        private string m_url;
        private string m_hash;
        private Image m_image;
        private SpriteRenderer m_spriteRenderer;
        private Texture2D m_texture2D;
        private CancellationToken m_cancellationToken;

        private Action<string> OnError;
        private Action<Sprite> OnCompleted;
        private Action<Texture2D> OnCompletedTexture2D;

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

        public ImageDownloader WithCompleted(Action<Texture2D> action)
        {
            OnCompletedTexture2D = action;
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

        public ImageDownloader Into(Texture2D texture2D)
        {
            m_texture2D = texture2D;
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
                Error(new NullReferenceException("Url has not been set. Use Load function to set image url"));
                return;
            }

            try
            {
                Uri uri = new Uri(m_url);
                m_url = uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                Error(e);
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

        private void Error(long responseCode, string message)
        {
            PDebug.Error(message);
            if (SignalLocator.Get<NetworkErrorSignal>().HasListener())
            {
                SignalLocator.Get<NetworkErrorSignal>().Dispatch(responseCode);
            }
            OnError?.Invoke(message);
        }

        private void Error(Exception exception)
        {
            PDebug.Error(exception.Message);
            if (SignalLocator.Get<CommonErrorSignal>().HasListener())
            {
                SignalLocator.Get<CommonErrorSignal>().Dispatch(exception.Message);
            }
            OnError?.Invoke(exception.Message);
        }

        private string CreateMD5(string input)
        {
            return Encryption.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private async UniTask DownloadImage()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(m_url);
            
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            await UniTask.WaitUntil(() => operation.isDone || Application.internetReachability == NetworkReachability.NotReachable
                , cancellationToken: m_cancellationToken);
            
            if (request.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            {
                Error(request.responseCode, "Error while downloading: " + request.error);
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

            if (m_image != null || m_spriteRenderer != null)
            {
                LoadTextureToSprite();
            }
            else
            {
                LoadTextureToTexture2D(ref m_texture2D);
            }
        }

        private void LoadTextureToSprite(Texture2D texture = null)
        {
            LoadTextureToTexture2D(ref texture);
            if (texture == null)
            {
                Error(new NullReferenceException("[ImageDownloader] Target texture is null"));
                return;
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
            PDebug.Info("[ImageDownloader] Finish loading image from: " + m_url);
            OnCompleted?.Invoke(sprite);
            
            Dispose();
        }
        
        private void LoadTextureToTexture2D(ref Texture2D texture)
        {
            if (!File.Exists(SAVE_PATH + m_hash))
            {
                Error(new NullOrEmptyException("Load image file failed! "));
                return;
            }

            if (texture == null)
            {
                texture = new Texture2D(1, 1, TextureFormat.ARGB32, 1, true);
            }
            
            byte[] fileData = File.ReadAllBytes(SAVE_PATH + m_hash);
            texture.LoadImage(fileData);
            OnCompletedTexture2D?.Invoke(texture);
        }

        public void Dispose()
        {
            m_image = null;
            m_spriteRenderer = null;
            OnCompleted = null;
            OnCompletedTexture2D = null;
            OnError = null;
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}