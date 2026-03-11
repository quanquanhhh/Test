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

            stepDeltaProgress = 0.05f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);

            if (GlobalSetting.RuntimeConfig.PlayMode == EPlayMode.EditorSimulateMode ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {

            }
            else
            {

                CheckAndUpdateManifest(fsm);
            }
        }
        protected async void CheckAndUpdateManifest(IFsm<LauncherFsm> fsm)
        {
            var package = YooAssets.GetPackage(GlobalSetting.PackageName);
            var operation = package.RequestPackageVersionAsync();
            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                Debug.Log(" GET RequestPackageVersion Fail" + operation.Error);
                return;
            }
            Debug.Log($" GET RequestPackageVersion Success : {operation.PackageVersion} Name {operation.PackageName}");
            var checkManifest = package.UpdatePackageManifestAsync(GlobalSetting.ResourceVersion,60);
            await checkManifest.Task;
            if (operation.Status ==  EOperationStatus.Failed)
            {
                Debug.LogError("UpdateManifest:Error->:" + operation.Error);
            }

            if (operation.Status == EOperationStatus.Succeed)
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
