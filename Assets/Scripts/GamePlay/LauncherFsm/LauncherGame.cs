using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foundation;
using Foundation.FSM;
using Foundation.Storage;
using GameConfig;
using GamePlay.Storage;
using GamePlay.UIMain;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GamePlay.LauncherFsm
{
    public class LauncherGame : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 100f - fsm.Owner.accumulateProgress;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            LoadStorage();
            LoadConfig();
        }

        private void LoadConfig()
        {
            LoadGameJsonConfig();
        }

        private void LoadStorage()
        {
            List<StorageBase> storagebase =  new List<StorageBase>();
            storagebase.Add(new BaseInfo());
            StorageManager.Instance.Init(storagebase);
            
        }
        
        private async void LoadGameJsonConfig()
        {
            
            var done =  await LoadGamePlayJsonData();
            if (string.IsNullOrEmpty(done))
            {
                var textAsset = await AssetLoad.Instance.LoadAsset<TextAsset>("gameconfigs");
                // var textAsset =  Resources.Load<TextAsset>("Config/gameconfigs");
                if (textAsset == null)
                {
                    return;
                }

                done = textAsset.text;
            } 
            
            string json = ConfigCrypto.DecryptResourceText(done,"running123123");
            Debug.Log(json);
            
            ConfigPackage data = JsonConvert.DeserializeObject<ConfigPackage>(json);

            UIModule.Instance.ShowAsync<MainUI>();
        }
         
        
        private async UniTask<string> LoadGamePlayJsonData()
        {
            if (!GameViewComponent._loadingLuncher.LoadS3)
            {
                return string.Empty;
            }
            string result = "";
            
            string url = $"{GlobalSetting.MainHostServer}/{Application.version}/gameconfigs.txt";
            Debug.Log(url);
            UnityWebRequest request = null;

            try
            {
                request = UnityWebRequest.Get(url);
                request.SetRequestHeader("User-Agent", "Mozilla/5.0");
                request.timeout = (int)(2000);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    result = request.downloadHandler.text;
                }
                else
                { 
                    Debug.LogWarning($"Web load failed: {request.result}, code: {request.responseCode}");
                }
            }
            catch (Exception ex)
            {  
                Debug.LogWarning($"Exception: {ex.Message}");
            }

            return result;
        }
        
    }
}