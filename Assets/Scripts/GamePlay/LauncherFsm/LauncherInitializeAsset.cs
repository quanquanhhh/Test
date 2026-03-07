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
            InitializationOperation op = null;
            BetterStreamingAssets.Initialize();
            YooAssets.Initialize();
            
            var package = YooAssets.CreatePackage(GlobalSetting.PackageName);
            YooAssets.SetDefaultPackage(package);
            if (LoadingLuncher.RunPlayMode == EPlayMode.EditorSimulateMode)
            {
                var param = new EditorSimulateModeParameters();
                param.EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(GlobalSetting.PackageName);
                op = package.InitializeAsync(param);
            }
            else
            {
                string defaultHostServer = GlobalSetting.GetHostResUrl(); 
                string fallbackHostServer =  GlobalSetting.GetHostResUrl(); 
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                
                var online = new HostPlayModeParameters();  
                online.BuildinFileSystemParameters = buildinFileSystemParams; 
                online.CacheFileSystemParameters = cacheFileSystemParams;
                op = package.InitializeAsync(online);
            }

            await op.Task;
            if (op.Status != EOperationStatus.Succeed)
            {
                Debug.LogError("LauncherInitializeAsset Failed" + op.Error);
            }
            ChangeState<LauncherCheckManifest>(fsm);
        }
    }
}