using UnityEngine;
using YooAsset;

namespace GamePlay
{
    [System.Serializable]
    public class YooAssetRuntimeConfig
    {
        public string PackageName = "Tile";
        public string ResourceVersion = "v1";
        public EPlayMode PlayMode = EPlayMode.HostPlayMode; 
        public string GetMainHostServerURL()
        {
            return BuildServerUrl(GlobalSetting.MainHostServer);
        }

        public string GetFallbackHostServerURL()
        {
            if (string.IsNullOrEmpty(GlobalSetting.FallbackHostServer))
            {
                return BuildServerUrl(GlobalSetting.MainHostServer);
            }
            return BuildServerUrl(GlobalSetting.FallbackHostServer);
        }

        private string BuildServerUrl(string serverRoot)
        {
            return $"{serverRoot}/{ResourceVersion}";
        }
    }

    public static class GlobalSetting
    {
        
        public static string MainHostServer = "https://tile.desiregirls.net/Test";
        public static string FallbackHostServer = "https://tile.desiregirls.net/Test";
        
        private static YooAssetRuntimeConfig _runtimeConfig = new YooAssetRuntimeConfig();

        public static YooAssetRuntimeConfig RuntimeConfig => _runtimeConfig;
        public static string PackageName => _runtimeConfig.PackageName;

        public static string ResourceVersion
        {
            get => _runtimeConfig.ResourceVersion;
            set => _runtimeConfig.ResourceVersion = value;
        }

        public static void Configure(YooAssetRuntimeConfig runtimeConfig)
        {
            if (runtimeConfig == null)
            {
                Debug.LogError("[YooAsset] Runtime config is null. Keep previous config.");
                return;
            }
            _runtimeConfig = runtimeConfig;
            Debug.Log($"[YooAsset] Config updated. Package:{_runtimeConfig.PackageName}, PlayMode:{_runtimeConfig.PlayMode}, MainHost:{_runtimeConfig.GetMainHostServerURL()}, FallbackHost:{_runtimeConfig.GetFallbackHostServerURL()}");
        }

        public static string GetHostResUrl(bool useFallback = false)
        {
            return useFallback
                ? _runtimeConfig.GetFallbackHostServerURL()
                : _runtimeConfig.GetMainHostServerURL();
        }
    }
}
