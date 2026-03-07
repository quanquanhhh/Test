using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Foundation;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;
using Event = Foundation.Event;

namespace GamePlay
{
    public class LoadingLuncher : MonoBehaviour
    {
        public string PackageName;
        public bool Local = true;
        public bool LoadS3 = true;
        public bool RunAccountCheck = true;
        private int FrameRate = 60;
        private float MAXCHECKTIME = 0;

        public static EPlayMode RunPlayMode = EPlayMode.EditorSimulateMode;
        
        private LauncherFsm.LauncherFsm launcherFsm;
        private void Start()
        {
            StartAsync().Forget();
            StartLauncher();
        }

        private void StartLauncher()
        {
            
            launcherFsm = new LauncherFsm.LauncherFsm();
            launcherFsm.Initialize();
            launcherFsm.Start();
            
        }

        private void Update()
        {
            if (launcherFsm != null)
            {
                launcherFsm.Update();
            }
        }

        private async UniTask StartAsync()
        {
            ConfigSelect.GetUseConfig();
            Application.targetFrameRate = FrameRate;
            Application.runInBackground = true;
            GameCommon.GameName = PackageName;
            RootManager.MgrRoot = GameObject.Find("Mgr");
            RootManager.UIRoot = GameObject.Find("Canvas").transform; 
            
            DontDestroyOnLoad(RootManager.MgrRoot);
            ScreenUtility.CheckRate();
            if (!RunAccountCheck && !LoadS3)
            {
                MAXCHECKTIME = 1;
            }
            InitilizationSDK();  
            await UniTask.NextFrame();
            if (!Local)
            {
                GameCommon.InitSDKsByPlatform();
            }
            await UniTask.NextFrame();
             
              
            
            CreateGame();
             

        }
 
 
        private async void CreateGame()
        {
            CreateUI();
            
            await UniTask.NextFrame(); 
            CreatePlay();
        }

        private async void CreatePlay()
        {
            
        }

        private void CreateUI()
        { 
        }
  
        private async UniTask<string> LoadGameConfigFromWeb()
        {
            if (!LoadS3)
            {
                return "";
            }
            string result = "";
            string url = $"https://she.sheriffbunny.net/GP/{Application.version}/TidesSolitaire.txt";
            // Debug.Log(url);
            UnityWebRequest request = null;

            try
            {
                request = UnityWebRequest.Get(url);
                request.SetRequestHeader("User-Agent", "Mozilla/5.0");
                request.timeout = (int)MAXCHECKTIME;
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
        
 
        private void InitilizationSDK()
        {
            AdMgr.Instance.OnInitMaxSDK();
        }
        
    }
}