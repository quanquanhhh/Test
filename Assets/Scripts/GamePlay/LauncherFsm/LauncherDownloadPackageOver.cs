using Foundation.FSM;
using YooAsset;

namespace GamePlay.LauncherFsm
{
    public class LauncherDownloadPackageOver : LauncherBase
    {
        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            var package = YooAssets.GetPackage(GlobalSetting.PackageName);
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            operation.Completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperationBase obj)
        {
            throw new System.NotImplementedException();
        }
    }
}