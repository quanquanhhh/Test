using Foundation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YooAsset;

namespace GamePlay.Component
{
    [RequireComponent(typeof(RawImage))]
    [DisallowMultipleComponent]
    public class UguiMediaSource : MonoBehaviour
    { 

        private RawImage _rawImage;
        private VideoPlayer _videoPlayer;
        private RenderTexture _renderTexture;

        private bool _isVideo;

        private void Awake()
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            _rawImage = GetComponent<RawImage>();
            _videoPlayer = GetComponent<VideoPlayer>();
            if (_videoPlayer == null)
                _videoPlayer = gameObject.AddComponent<VideoPlayer>();

            _renderTexture = new RenderTexture((int)ViewUtility.DesignSize.x, (int)ViewUtility.DesignSize.y, 0, RenderTextureFormat.ARGB32);
            _renderTexture.name = $"{gameObject.name}_MediaRT";
            _renderTexture.Create();

            _videoPlayer.playOnAwake = false;
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.skipOnDrop = true;
            _videoPlayer.isLooping = true;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.errorReceived += OnVideoErrorReceived;
        }

        private void OnDestroy()
        {
            _videoPlayer.prepareCompleted -= OnVideoPrepared;
            _videoPlayer.errorReceived -= OnVideoErrorReceived;

            if (_videoPlayer.isPlaying)
                _videoPlayer.Stop();

            _videoPlayer.targetTexture = null;
            _videoPlayer.url = string.Empty;

            if (_renderTexture != null)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
                _renderTexture = null;
            }
        }

        public void SetSource(string sourceName, bool isVideo)
        {
            _isVideo = isVideo;

            if (isVideo)
                LoadVideo(sourceName);
            else
                LoadImage(sourceName);
        }

        private async void LoadImage(string sourceName)
        {
            StopVideoInternal(); 

            Texture texture = await AssetLoad.Instance.LoadAsset<Texture>(sourceName);
            if (texture == null)
            {
                Debug.LogError($"[UguiMediaSource] Load image failed : {sourceName}", this);
                _rawImage.texture = null;
                return;
            }


            _rawImage.texture = texture;
            ApplyAspect(texture.width, texture.height);
        }

        private async void LoadVideo(string sourceName )
        {
            StopVideoInternal(); 
            var video = await AssetLoad.Instance.LoadAsset<VideoClip>(sourceName);   

            _rawImage.texture = _renderTexture;

            _videoPlayer.source = VideoSource.VideoClip; 
            _videoPlayer.clip = video;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
            _videoPlayer.Prepare();
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            if (source == null || !_isVideo)
                return;

            _rawImage.texture = _renderTexture;

            int width = (int)source.width;
            int height = (int)source.height;
            if (width > 0 && height > 0)
                ApplyAspect(width, height);

            source.Play();
        }

        private void OnVideoErrorReceived(VideoPlayer source, string message)
        {
            Debug.LogError($"[UguiMediaSource] Video error : {message}", this);
        }

        private void StopVideoInternal()
        {
            if (_videoPlayer == null)
                return;

            if (_videoPlayer.isPlaying)
                _videoPlayer.Stop();

            _videoPlayer.url = string.Empty;

            if (_renderTexture != null)
            {
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = _renderTexture;
                GL.Clear(false, true, Color.clear);
                RenderTexture.active = active;
            }
        }
 

        private void ApplyAspect(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            RectTransform rt = _rawImage.rectTransform;
            float currentHeight = rt.sizeDelta.y;
            if (currentHeight <= 0f)
                return;

            float newWidth = currentHeight * width / (float)height;
            rt.sizeDelta = new Vector2(newWidth, currentHeight);
        }
    }
}