using Foundation.FSM;
using UnityEngine;
using YooAsset;

namespace GamePlay.LauncherFsm
{
    /// <summary>
    /// FsmRequestPackageVersion
    /// </summary>
    public class LauncherCheckManifest : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {

            stepDeltaProgress = 10f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);

            CheckAndUpdateManifest(fsm);
        }
        protected async void CheckAndUpdateManifest(IFsm<LauncherFsm> fsm)
        {
            var package = YooAssets.GetPackage(GlobalSetting.PackageName);
            string checkVersion = GlobalSetting.ResourceVersion;
            if (GlobalSetting.RuntimeConfig.PlayMode == EPlayMode.EditorSimulateMode ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                var operation = package.RequestPackageVersionAsync();
                await operation.Task;
                checkVersion = operation.PackageVersion;
            }
            else
            {
                var a= package.ClearCacheFilesAsync(EFileClearMode.ClearAllManifestFiles);
                await a.Task;
            }

            var checkManifest = package.UpdatePackageManifestAsync(checkVersion,60);
            await checkManifest.Task;
            if (checkManifest.Status ==  EOperationStatus.Failed)
            {
                Debug.LogError("UpdateManifest:Error->:" + checkManifest.Error);
            }

            if (checkManifest.Status == EOperationStatus.Succeed)
            {
                ChangeState<LauncherDownload>(fsm);
                var packageVersion = YooAssets.GetPackage(GlobalSetting.PackageName).GetPackageVersion();
                if (!string.IsNullOrEmpty(packageVersion))
                {
                    PlayerPrefs.SetString("ResourceVersionKey", packageVersion);
                    Debug.Log("Store Local Res Version:" + packageVersion);
                }
            }
            else
            {
                ChangeState<LauncherGame>(fsm);
            }
        }
    }
}
