using System;
using DG.Tweening;
using Foundation.FSM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.LauncherFsm
{
    public class LauncherFsm
    {
        
        private IFsmManager _fsmManager;
        private IFsm<LauncherFsm> _fsm;

        // public TextMeshProUGUI debugText;

        public float accumulateProgress = 0.0f;
        public float displayProgress = 0.0f;

        public string debugInfo;
        public GameObject LoadingView;
        
        private float _progressVelocity = 0f;
        private const float SmoothTime = 0.25f;   // 越小越快，越大越柔和
        private const float MaxSpeed = 200f;


        public Type[] fsmStateTypes =
        {
            typeof(LauncherSetUpView),
            typeof(LauncherGetResourceVersion),
            typeof(LauncherInitializeAsset),
            typeof(LauncherCheckManifest),
            typeof(LauncherDownload),
            typeof(LauncherUpdateOver),
            typeof(LauncherGame),
        };

        private DOTween _doTween;

        public void ShowInitializeText(float progress)
        {
            if (LoadingView != null)
            {
                var slider = LoadingView.transform.Find("ProgressSlider").GetComponent<Slider>();
                var textMeshProugui = LoadingView.transform.Find("ProgressSlider/ProgressText")
                    .GetComponent<TextMeshProUGUI>();
                float clampedProgress = Mathf.Clamp(progress, 0f, 100f);
                slider.value = clampedProgress / 100;
                textMeshProugui.text = $"{Mathf.FloorToInt(clampedProgress)}%";
            }
        }

        public void ShowDebugInfo(string info)
        {
            
            if (LoadingView != null)
            { 
                var textMeshProugui = LoadingView.transform.Find("ProgressSlider/DebugInfo")
                    .GetComponent<TextMeshProUGUI>();
                textMeshProugui.text = info;
            }
        }
        
        public void Initialize()
        {
            _fsmManager = new FsmManager();

            var fsmStates = new FsmState<LauncherFsm>[fsmStateTypes.Length];

            for (var i = 0; i < fsmStateTypes.Length; i++)
            {
                fsmStates[i] = (FsmState<LauncherFsm>) Activator.CreateInstance(fsmStateTypes[i]);
            }

            _fsm = _fsmManager.CreateFsm("Launcher", this, fsmStates);
        }
        public void Start() 
        {
            if (_fsm == null)
            {
                throw new UnityException("You must initialize procedure first.");
            }
            
            _fsm.Start<LauncherSetUpView>();
        }
        
        public void Update()
        {
            if (_fsm == null || LoadingView == null)
            {
                return;
            }

            
            accumulateProgress = Mathf.Clamp(accumulateProgress, 0f, 100f);
            displayProgress = Mathf.SmoothDamp(
                displayProgress, 
                accumulateProgress, 
                ref _progressVelocity,
                SmoothTime,
                MaxSpeed,
                Time.unscaledDeltaTime);
                    
            if (Mathf.Abs(displayProgress - accumulateProgress) < 0.05f)
            {
                displayProgress = accumulateProgress;
            }
            ShowInitializeText(displayProgress);
            if (!string.IsNullOrEmpty(debugInfo))
            {
                ShowDebugInfo(debugInfo);
            }

            if (Mathf.Approximately(displayProgress, 100) && Mathf.Approximately(accumulateProgress, 100) )
            {
                //准备关闭loading
                LoadingView.GetComponent<CanvasGroup>().DOFade(0, 0.35f).SetEase(Ease.OutSine).OnComplete((() =>
                {
                    if (LoadingView != null)
                    {
                        GameObject.Destroy(LoadingView);
                    }
                }));
            }
            
        }

    }
}