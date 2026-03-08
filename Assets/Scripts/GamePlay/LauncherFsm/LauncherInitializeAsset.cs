using Foundation;
using Foundation.FSM;
using UnityEngine;
using YooAsset;

namespace GamePlay.LauncherFsm
{
    public class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    public class LauncherInitializeAsset : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 0.08f;
            base.OnInit(fsm);
        }

        protected internal override async void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            BetterStreamingAssets.Initialize();
            YooAssets.Initialize();

            var package = YooAssets.TryGetPackage(GlobalSetting.PackageName) ?? YooAssets.CreatePackage(GlobalSetting.PackageName);
            YooAssets.SetDefaultPackage(package);

            InitializationOperation operation = CreateInitializeOperation(package);
            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError("[YooAsset] LauncherInitializeAsset failed : " + operation.Error);
                return;
            }

            ResourceModule.Instance.Initialize(GlobalSetting.PackageName);
            Debug.Log($"[YooAsset] Package initialize succeed. Package:{GlobalSetting.PackageName}");
            ChangeState<LauncherCheckManifest>(fsm);
        }

        private InitializationOperation CreateInitializeOperation(ResourcePackage package)
        {
            var runtimeConfig = GlobalSetting.RuntimeConfig;
            Debug.Log($"[YooAsset] Initializing package. Mode:{runtimeConfig.PlayMode}, Package:{runtimeConfig.PackageName}");

            if (runtimeConfig.PlayMode == EPlayMode.EditorSimulateMode)
            {
                var editorParams = new EditorSimulateModeParameters
                {
                    EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(runtimeConfig.PackageName)
                };
                return package.InitializeAsync(editorParams);
            }

            if (runtimeConfig.PlayMode == EPlayMode.OfflinePlayMode)
            {
                var offlineParams = new OfflinePlayModeParameters
                {
                    BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters()
                };
                return package.InitializeAsync(offlineParams);
            }

            var defaultHostServer = runtimeConfig.GetMainHostServerURL();
            var fallbackHostServer = runtimeConfig.GetFallbackHostServerURL();
            Debug.Log($"[YooAsset] Host URLs. Main:{defaultHostServer}, Fallback:{fallbackHostServer}");

            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var hostParams = new HostPlayModeParameters
            {
                BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters(),
                CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices)
            };
            return package.InitializeAsync(hostParams);
        }
    }
}
