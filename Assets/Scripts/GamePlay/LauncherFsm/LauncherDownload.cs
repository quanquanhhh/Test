using Foundation;
using Foundation.FSM;
using UnityEngine;
using YooAsset;

namespace GamePlay.LauncherFsm
{
    public class LauncherDownload: LauncherBase
    {
        private int downloadCount = 0;
        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);

            var downloader = ResourceModule.Instance.CreateDownloader();
            var downloader2 = ResourceModule.Instance.CreateDownloader("Hot");
            var a = downloader2.TotalDownloadCount;
            if (downloader.TotalDownloadCount != 0)
            {
                downloadCount = downloader.TotalDownloadCount;
                long totaldownloadBytes = downloader.TotalDownloadBytes;
                float sizeMb = totaldownloadBytes / (1024 * 1024);
                sizeMb = Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                float totalSize = sizeMb;
                BeginDownload(downloader);
            }
        }
        private async void BeginDownload(ResourceDownloaderOperation downloader)
        { 
            // 注册下载回调 
            downloader.DownloadErrorCallback = OnDownloadErrorCallback;
            downloader.DownloadUpdateCallback = OnDownloadProgressCallback;
            downloader.DownloadFinishCallback = OnDownloadFinishCallback;
            downloader.DownloadFileBeginCallback = OnDownloadFileBeginCallback;
            downloader.BeginDownload();
            await downloader.Task;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;
 
            // OnAssetsLoadCompleted();
        }

        private void OnDownloadFileBeginCallback(DownloadFileData data)
        {
            Debug.Log(data);
        }

        private void OnDownloadFinishCallback(DownloaderFinishData data)
        {
            Debug.Log(data);
        }

        private void OnDownloadProgressCallback(DownloadUpdateData data)
        {
            Debug.Log(" oooo ::");
        }

        private void OnDownloadErrorCallback(DownloadErrorData   error)
        {
            Debug.Log(" error ::");
        }

    }
}