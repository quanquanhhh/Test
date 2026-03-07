using AdjustSdk;
using DG.Tweening;
using UnityEngine;

namespace Foundation
{
    public class AdjustManager : SingletonComponent<AdjustManager>
    {
        public string mediaSource;
        public int AdjustStatus = -1; // -1 没返回  0 natualuser 1 aduser
        public bool AdjustBack = false;

        private string useKey;
        public void InitAdjust()
        {
#if UNITY_IOS
            Adjust.RequestAppTrackingAuthorization(b => Debug.Log(b));
#endif
            Adjust.AddGlobalCallbackParameter("customer_user_id", SystemInfo.deviceUniqueIdentifier);
            
            AdjustConfig adjustConfig = new AdjustConfig(ConfigSelect.AdjustDevKey, AdjustEnvironment.Production);
#if DEBUG_APP || UNITY_EDITOR
            adjustConfig.LogLevel = AdjustLogLevel.Verbose;
#else
            adjustConfig.LogLevel = AdjustLogLevel.Suppress;
#endif
            adjustConfig.AttributionChangedDelegate = OnAttributionChanged;
            adjustConfig.SessionSuccessDelegate = OnSessionSuccess;
            adjustConfig.SessionFailureDelegate = OnSessionFailure;
            adjustConfig.EventFailureDelegate = OnEventFailureDelegate;
            adjustConfig.EventSuccessDelegate = OnEventSuccessDelegate;
            DOVirtual.DelayedCall(1f, delegate()
            {
                Adjust.InitSdk(adjustConfig);
            });
        }
        private void OnEventSuccessDelegate(AdjustEventSuccess obj)
        {
            
        }
        private void OnEventFailureDelegate(AdjustEventFailure obj)
        {
        }
        private void OnSessionSuccess(AdjustSessionSuccess pRet)
        {
        }

        private void OnSessionFailure(AdjustSessionFailure pRet)
        {
        }
        private void OnAttributionChanged(AdjustAttribution adjustAttribution)
        { 
            Debug.Log(" [TODO-------ADJUST] OnAttributionChanged" + adjustAttribution.Network.ToLower() + " || " +  adjustAttribution.TrackerName + " || " +  Application.identifier);
            
            mediaSource = adjustAttribution.Network;
            if (adjustAttribution.Network.ToLower().Equals("organic")
                || adjustAttribution.TrackerName.ToLower().Equals("organic")
                || adjustAttribution.Network.ToLower().Contains("no user consent")
                || adjustAttribution.TrackerName.ToLower().Equals("no user consent"))
            {
           
                if (AdjustStatus == -1)
                {
                    AdjustStatus = 0;
                    Event.Instance.SendEvent(new AdjustEventSuccess());
                    Event.Instance.SendEvent(new ShowSystemTips("Adjust Success "+ mediaSource, true));
                }
            }
            else
            {
                AdjustStatus = 1;
                Event.Instance.SendEvent(new AdjustEventSuccess());
                Event.Instance.SendEvent(new ShowSystemTips("Adjust Success "+ mediaSource, true));
            }
            AdjustBack = true;
        }
        public void LogAdjustRevenue(MaxSdkBase.AdInfo adInfo)
        {
            AdjustAdRevenue adRevenue = new AdjustAdRevenue("applovin_max_sdk");
            adRevenue.SetRevenue(adInfo.Revenue, "USD");
            adRevenue.AdRevenueNetwork = adInfo.NetworkName;
            adRevenue.AdRevenueUnit = adInfo.AdUnitIdentifier;
            adRevenue.AdRevenuePlacement = adInfo.Placement;     
            
#if UNITY_IOS
                adRevenue.AddCallbackParameter("platform", "ios");
#elif UNITY_ANDROID
            adRevenue.AddCallbackParameter("platform", "android");
#else
            adRevenue.AddCallbackParameter("platform", "other");
#endif      
            Adjust.TrackAdRevenue(adRevenue);
        }
        public void LogAdjustRevenue2(float timer)
        {
            AdjustAdRevenue adRevenue = new AdjustAdRevenue("applovin_max_sdk");
            adRevenue.SetRevenue(timer, "USD"); 

#if UNITY_IOS
                adRevenue.AddCallbackParameter("platform", "ios");
#elif UNITY_ANDROID
            adRevenue.AddCallbackParameter("platform", "android");
#else
            adRevenue.AddCallbackParameter("platform", "other");
#endif      
            Adjust.TrackAdRevenue(adRevenue);
        }
        
        
    }
}