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

        public GameObject LoadingView;

        public Type[] fsmStateTypes =
        {
            typeof(LauncherCheckManifest),
            typeof(LauncherGame),
            typeof(LauncherInitializeAsset),
            typeof(LauncherGetResourceVersion),
            typeof(LauncherSetUpView),
            typeof(LauncherShowLoading),
        };

        private DOTween _doTween;

        public void ShowInitializeText(float initprogress)
        {
            if (LoadingView != null)
            {
                var slider = LoadingView.transform.Find("ProgressSlider").GetComponent<Slider>();
                var textMeshProugui = LoadingView.transform.Find("ProgressSlider/ProgressText")
                    .GetComponent<TextMeshProUGUI>();
                slider.value = initprogress;
                textMeshProugui.text = initprogress + "%";
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
            if (_fsm != null)
            {
                if (LoadingView != null && !(_fsm.CurrentState is LauncherGame))
                {
                    if (displayProgress < accumulateProgress)
                    {
                        displayProgress = displayProgress + (accumulateProgress - displayProgress) * 0.1f;
                    }
                     
                    ShowInitializeText(displayProgress);
                }
                 
                _fsmManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

    }
}