using UnityEngine;

namespace Foundation
{
    public enum TaskType
    {
        Spin = 0,
        WatchingVideos = 1,
        TriggerBigWin = 2,
        WinAmountOnBonus = 3,
        TriggerJackpot = 4,
        InQueue = 5,
        WaitForReview = 6
    }
    
    public enum MonitorType
    {
        None,
        RVCounter,
        RvLimited
    }
    public class GameCommon
    {
        public static string GameName;
        public static bool PlayerFirstOpenApp = false ;
        public static bool AdSwitch = false; //资源包
        public static bool IsCoin = false;
        public static void InitSDKsByPlatform(MonitorData monitorData = null)
        {

#if UNITY_IOS||UNITY_EDITOR
            AdjustManager.Instance.InitAdjust();
#elif UNITY_ANDROID
            if (monitorData == null)
            {
                Debug.LogError( " [Monitor] is null." );
            }
            MonitorManager.Instance.InitMonitorData(monitorData);
            MonitorManager.Instance.InitMonitor();

#endif
        }

        public static MonitorType CheckMonitor()
        {

#if UNITY_ANDROID

            if (MonitorManager.IsInitMonitor)
            {
                if ( MonitorUtil.RVCounter >= MonitorUtil.RvLimitCount)
                { 
                    return MonitorType.RVCounter;
                }
                else if (MonitorUtil.RvLimited && MonitorUtil.IVLimited)
                {
                     
                    return MonitorType.RvLimited;
                }
            } 
#endif
            return MonitorType.None;
        }
    }
}