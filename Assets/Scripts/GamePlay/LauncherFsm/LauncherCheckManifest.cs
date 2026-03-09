using Foundation.FSM;
using UnityEngine;
using YooAsset;

namespace GamePlay.LauncherFsm
{
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
            Debug.Log("CheckAndUpdatePatchManifest");
 
            var package = YooAssets.GetPackage(GlobalSetting.PackageName);
             
            
            
            // package.UnloadAllAssetsAsync();

            Debug.Log($"CheckServerVersion Pass"); 

            var operation = package.UpdatePackageManifestAsync(GlobalSetting.ResourceVersion,60);

            await operation.Task;

            var a =package.GetAllAssetInfos();
            Debug.Log($"UpdateManifest Pass");

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
