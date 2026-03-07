using System;
using System.Diagnostics.Tracing; 
using UnityEngine;

namespace Foundation
{
    
    public class AdMgr : SingletonComponent<AdMgr>
    {
        private AdMaxOp _adMaxOp;
        public bool isPlayAd = false;
        private readonly float CHECKMAXTIME = 3f;
        private bool needIV = false;
        public void OnInitMaxSDK()
        {
            _adMaxOp =  new AdMaxOp();
            _adMaxOp.OnInitMaxSDK();
        }
        
        
        public void CloseMask()
        {
            
            if (UIModule.Instance.Get<UIMaskLoading>() != null)
            {
                UIModule.Instance.Close<UIMaskLoading>();
            }
        }
        public void PlayRV(Action success, string pos, Action fail = null)
        {
            if (isPlayAd)
            { 
                Debug.Log("[ TEST ] 4");
                return;
            }

            Action successBack = () =>
            {
                CloseMask(); 
                isPlayAd = false;
                Event.Instance.SendEvent(new RVPlayFinished(success));
                // SaveStorage.Instance.AddRvProtectIv();
                // bool cotinueplay = SaveStorage.Instance.IsContinuePlayIv();
                // if (cotinueplay)
                // {
                //     SaveStorage.Instance.ResetIntervalRvForIv();
                //     PlayIV(success, pos);
                // }
                // else
                // {
                //     success?.Invoke();
                // }
                Event.Instance.SendEvent(new EventTask((int)TaskType.WatchingVideos));
            };

            MonitorType checkMonitor = GameCommon.CheckMonitor();
            switch (checkMonitor)
            {
                case MonitorType.None:
                    break;
                case MonitorType.RVCounter:
                    // UIModule.Instance.ShowAsync<AdLimit>();
                    fail?.Invoke();
                    isPlayAd = false;
                    return;
                case MonitorType.RvLimited:
                    
                    Event.Instance.SendEvent(new ShowSystemTips("Advertisement loading failed.")); 
                    fail?.Invoke();
                    isPlayAd = false;
                    return;
            }
#if UNITY_EDITOR
            Event.Instance.SendEvent(new ShowSystemTips("Get Ad Reward - Mock"));
            successBack?.Invoke();
            return;
#endif
            
            if (!GameCommon.AdSwitch)
            {
                Event.Instance.SendEvent(new ShowSystemTips("Get Ad Reward - Mock"));
                successBack?.Invoke();
                return;
            }
            isPlayAd = true;
            TryPlayRv(successBack, fail, pos);
        }


        public void PlayIV(Action success,  string pos)
        {
            if (isPlayAd || GameCommon.IsCoin)
            {
                success?.Invoke();
                return;
            }

            if (MonitorUtil.IVCounter >= MonitorUtil.IvLimitCount ||
                (MonitorUtil.IVLimited && MonitorUtil.RvLimited))
            { 
                success?.Invoke();
                return;
            }
            isPlayAd = true;
            Action ABCD = () =>
            { 
                isPlayAd = false;
                success?.Invoke();
                Event.Instance.SendEvent(new EventTask((int)TaskType.WatchingVideos));
                Event.Instance.SendEvent(new IVPlayFinished());
                
            };
       
#if UNITY_EDITOR
            Event.Instance.SendEvent(new ShowSystemTips("Play IV - Mock"));
            ABCD?.Invoke();
            return;
#endif     
            if (!GameCommon.AdSwitch)
            {
                Event.Instance.SendEvent(new ShowSystemTips("Play IV - Mock"));
                ABCD?.Invoke();
                return;
            }
            TryPlayIv(ABCD, pos + "_IV");
        }

        public void PlayIVFromAppComeBack(Action callback)
        {
            if (isPlayAd || GameCommon.IsCoin)
            {
                callback?.Invoke();
                return;
            }
#if UNITY_EDITOR
            Event.Instance.SendEvent(new ShowSystemTips("Play IV FromAppComeBack - Mock")); 
            return;
#endif     

            if (!GameCommon.AdSwitch)
            {
                Event.Instance.SendEvent(new ShowSystemTips("Play IV FromAppComeBack - Mock")); 
                return;
            }
            
            bool isReady = _adMaxOp.IsRVReady();
            
            if (isReady)
            {
                Debug.Log( " [IV]  play   3  "  );
                
                _adMaxOp.ShowInterstitiaAd(callback, "AppComeBack_IV");
            }
            
        }
        private async void TryPlayRv(Action success, Action fail, string pos)
        {
            float time = 0;
            bool isReady = _adMaxOp.IsRVReady();
            if (!isReady)
            {
                UIModule.Instance.ShowAsync<UIMaskLoading>();
                while (time < CHECKMAXTIME && !isReady)
                {
                    time += Time.deltaTime;
                    isReady = _adMaxOp.IsRVReady();
                    await UniTaskMgr.Instance.Yield();
                }
                CloseMask();
            }


            if (isReady)
            {
                _adMaxOp.ShowRewardedAd(success, pos+"_RV");
            }
            else if (_adMaxOp.IsIVReady())
            { 
                _adMaxOp.ShowInterstitiaAd(success, pos+"_IV");
            }
            else
            {
                Event.Instance.SendEvent(new ShowSystemTips("Sorry, the video AD is not ready !"));
                fail?.Invoke();
                isPlayAd = false;
            }
        }
        private async void TryPlayIv(System.Action callBack, string pos)
        {
            float time = 0;
            bool isReady = _adMaxOp.IsIVReady(); 

            if (isReady)
            { 
                // SaveStorage.Instance.AddIvProtectIv(); 
                _adMaxOp.ShowInterstitiaAd(callBack, pos);
                
            }
            else
            {
                callBack?.Invoke();
                isPlayAd = false;
            }
        }
        
    }
}