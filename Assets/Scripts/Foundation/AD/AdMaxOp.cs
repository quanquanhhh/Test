using System;
using System.Collections.Generic;
using System.Text; 
using UnityEngine;

namespace Foundation
{
    public class AdMaxOp
    {
        
        protected System.Action mRvCallBack = null;  
        protected System.Action mIvCallBack = null;  
        
        public void OnInitMaxSDK()
        {
            MaxSdk.SetHasUserConsent(true); 

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += InterstitialFailedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
                MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

                LoadRewardAd();
                LoadInterstitial();
            };
            MaxSdk.InvokeEventsOnUnityMainThread = true;
            MaxSdk.SetSdkKey(GetSDKKEY());
            MaxSdk.InitializeSdk();
        }
        public void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(GetIvId());
            StatisticsManager.Instance.SendLogEvent("ad_request");
        }

        public void LoadRewardAd()
        {
            MaxSdk.LoadRewardedAd(GetRvId());
            StatisticsManager.Instance.SendLogEvent("ad_request");
        }
        public string GetSDKKEY()
        {
            string str =
                "T0RhN0NaWFhVbGxLVEJiMzRYZ0pVQUdxX1JmcXpHQnkxX09pdzI0ZlhaUmRCQUV0UEhTTkZIM0NweE1xbGdrWjZJaU1EeFY5Yk11RjRnMDM2blYxN00=";
            //这儿需要加密
            return Decrypt(str);
        }
        public static string Decrypt(string cipherText)
        {
            var bytes = Convert.FromBase64String(cipherText);
            return Encoding.UTF8.GetString(bytes);
        }
        public string GetRvId()
        {
            return ConfigSelect.AdRewardUnit; 
        }

        public string GetIvId()
        {
            return ConfigSelect.AdInterstitialUnit; 
        }
        
          #region IV回调
        //--插屏回调
        /// <summary>
        /// 插屏加载初始化
        /// </summary>

        int mRetryAttemptI = 0;
        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var dic = new Dictionary<string, object>();
            dic["posId"] = adInfo.Placement;
            StatisticsManager.Instance.SendLogEvent("ad_click", dic);
        } 
        /// <summary>
        /// 插屏加载成功回调
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            StatisticsManager.Instance.SendLogEvent("ad_fill");
            mRetryAttemptI = 0;
        }
        /// <summary>
        /// 插屏初始化失败
        /// 重新初始化
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void InterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            mRetryAttemptI++;
            double retryDelay = System.Math.Pow(2, System.Math.Min(6, mRetryAttemptI));
            DelayLoadInterstitialAdAsync((float)retryDelay);
        }
        private async void DelayLoadInterstitialAdAsync(float delayTime)
        {
            // await new WaitForSeconds(delayTime);
            await UniTaskMgr.Instance.WaitForSecond(delayTime);
            LoadInterstitial();
        }

        /// <summary>
        /// 插屏播放失败
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            IvCallBack();
            LoadInterstitial();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IvFailedEvent", $"{adInfo.Placement}-{errorInfo.Message}");
            StatisticsManager.Instance.SendLogEvent("show_fail", dic);
        }
        /// <summary>
        /// 插屏播放成功回调
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
#if UNITY_ANDROID 
            MonitorUtil.OnPlayVideoEnd(1, adInfo.NetworkName);
#endif
            IvCallBack();
            LoadInterstitial();
            
            if (UIModule.Instance.Get<UIMaskLoading>() != null)
                UIModule.Instance.Close<UIMaskLoading>();
        }
        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }
        #endregion

        #region RV回调
        int mRetryAttemptR = 0;
        /// <summary>
        /// 奖励视频初始化成功回调
        /// </summary>
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            mRetryAttemptR = 0;
            StatisticsManager.Instance.SendLogEvent("ad_fill");
        }
        private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        { 
            StatisticsManager.Instance.SendAdGetReward(adInfo);
#if UNITY_ANDROID 
            ShavingManager.Instance.LogAfAdRevenue(adInfo);
#else
            AdjustManager.Instance.LogAdjustRevenue(adInfo);
#endif
        }
        /// <summary>
        /// 奖励视频初始化失败回调
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            mRetryAttemptR++;
            double retryDelay = System.Math.Pow(2, System.Math.Min(6, mRetryAttemptR));

            DelayLoadRewardedAdAsync((float)retryDelay);
        }
        private async void DelayLoadRewardedAdAsync(float time)
        { 
            await UniTaskMgr.Instance.WaitForSecond(time);
            LoadRewardAd();
        }
        /// <summary>
        /// 奖励视频播放失败
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo adInfo)
        {
            LoadRewardAd();
            var dic = new Dictionary<string, object>();
            dic["posId"] = $"{adInfo.Placement}-{error.Message}";
            StatisticsManager.Instance.SendLogEvent("show_fail", dic);
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var dic = new Dictionary<string, object>();
            dic["posId"] = adInfo.Placement;
            StatisticsManager.Instance.SendLogEvent("ad_click", dic);
        }
        /// <summary>
        /// 奖励视频被关闭
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

            LoadRewardAd();
            RVCallBack();
#if UNITY_ANDROID
            MonitorUtil.OnPlayVideoEnd(0, adInfo.NetworkName);
#endif
            
            if (UIModule.Instance.Get<UIMaskLoading>() != null)
                UIModule.Instance.Close<UIMaskLoading>();
        }


        /// <summary>
        /// 获得奖励回调
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="reward"></param>
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {

        }
        #endregion

        #region 展示广告

        private string _rwPosId;
        private string _initPosId;
        /// <summary>
        /// 展示激励广告
        /// </summary>
        public void ShowRewardedAd(System.Action cb, string pos)
        {
#if UNITY_ANDROID
            ShavingManager.Instance.ShownAD();
#endif 
            this.mRvCallBack = cb;
            _rwPosId = pos + "_pv";
            var id = GetRvId();
            var dic = new Dictionary<string, object>();
            dic["posId"] = _rwPosId;
            StatisticsManager.Instance.SendLogEvent("ad_pass", dic);
            
#if UNITY_ANDROID
            MonitorUtil.OnPlayVideoStart();
#endif
            MaxSdk.ShowRewardedAd(id, _rwPosId);
        }
        //展示插屏广告
        public void ShowInterstitiaAd(System.Action cb, string pos)
        {
#if UNITY_ANDROID
            ShavingManager.Instance.ShownAD();
#endif 
            this.mIvCallBack = cb;
            var id = GetIvId();
            _initPosId = pos + "iv_close";
            var dic = new Dictionary<string, object>();
            dic["posId"] = _initPosId;
            StatisticsManager.Instance.SendLogEvent("ad_pass", dic);
            MaxSdk.ShowInterstitial(id, _initPosId);
        }

        #endregion

        protected void RVCallBack()
        {
            mRvCallBack?.Invoke();
            mRvCallBack = null;
        }
        protected void IvCallBack()
        { 
            mIvCallBack?.Invoke();
            mIvCallBack = null;
        }        
        public bool IsRVReady()
        {
            var id = GetRvId();
            return MaxSdk.IsRewardedAdReady(id);
        }

        public bool IsIVReady()
        {
            var id = GetIvId();
            return MaxSdk.IsInterstitialReady(id);
        }
    }
}