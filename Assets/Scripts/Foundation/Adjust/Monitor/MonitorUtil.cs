using System;
using System.Collections.Generic;
using UnityEngine; 

namespace Foundation
{
    public class MonitorUtil
    {
        // static string sKey =  GameCommon.GameName + "PlayerFirstCome";
        // public static int PlayerFirstOpenApp //0 first  other no first
        // {
        //     get
        //     {
        //         return PlayerPrefs.GetInt(sKey, 0);
        //     }
        //     set
        //     {
        //         PlayerPrefs.SetFloat(sKey, value);
        //     }
        // }

        // private static bool isFirstCome
        // {
        //     get
        //     {
        //         return GameCommon.PlayerFirstOpenApp == 0;
        //     }
        // }

        public static bool hasAnyTaskProgressing = false;
        //风控配置 从配置表读取值
        public static bool SwitchNumber; //数字联盟 配置表 numbers
        public static bool SwitchBehavior; //用户行为  配置表 behavior
        public static bool SwitchDevice; //设备环境 配置表device                 
        public static bool SwitchAdvInterval; //广告间隔  配置表ad_short
        public static bool SwitchFirstAdv; //第一个广告  配置表first_rv_time
        public static bool SwitchDeemAdv; //提现时广告 配置表  deem_ad            
        public static bool SwitchDeviceVpn; //配置表vpn
        public static bool SwitchDeviceRootPower; //配置表root
        public static bool SwitchDeviceCard; //配置表sim
        public static bool SwitchDeviceSimulator; //配置表simulator
        public static bool SwitchDeviceDevelopeMode; //配置表developer
        public static bool SwitchDeviceGoogle; //配置表googleplay
        public static int DeemLessCount = 3; //配置表deem_less
        public static int DeemMoreCount = 20; //配置表deem_more
        public static int RvLimitCount; //配置表 rvlimit

        public static int IvLimitCount; //配置表ivlimit

        //------------------------------------------------------------------------------//
        private static int rvCounter;
        private static int ivCounter;
        private static int shortShowCounter;
        private static int shortCloseCounter;
        private static long lastAdTime;
        private static long startAdTime;

        public static int RVCounter
        {
            get { return rvCounter; }
            set
            {
                rvCounter = value;
                PlayerPrefs.SetInt(MonitorAssetsEnum.assets_rv_counter.ToString(), rvCounter);
            }
        }

        public static int IVCounter
        {
            get { return ivCounter; }
            set
            {
                ivCounter = value;
                PlayerPrefs.SetInt(MonitorAssetsEnum.assets_iv_counter.ToString(), ivCounter);
            }
        }

        public static int ShortShowCounter
        {
            get { return shortShowCounter; }
            set
            {
                shortShowCounter = value;
                PlayerPrefs.SetInt(MonitorAssetsEnum.shortshow_counter.ToString(), shortShowCounter);
            }
        }

        public static int ShortCloseCounter
        {
            get { return shortCloseCounter; }
            set
            {
                shortCloseCounter = value;
                PlayerPrefs.SetInt(MonitorAssetsEnum.shortclose_Counter.ToString(), shortCloseCounter);
            }
        }

        public static bool RvLimited = false;

        public static bool IVLimited = false;

        //loading结束进游戏时调用
        public static void StartMonitorControl()
        {
            //设备相关风控检测
            int vpn = 0;
            int root = 0;
            int sim = 1;
            int simulator = 0;
            int developer = 0;
            int google = 0;
            //检测是否为googleplay下载
            if (!MonitorDataUtil.HasGpSource())
            {
                if (SwitchDevice && SwitchDeviceGoogle)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.googleplay);
                google = 0;
            }

            //vpn检测
            if (MonitorDataUtil.IsVpn())
            {
                if (SwitchDevice && SwitchDeviceVpn)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.vpn);
                vpn = 1;
            }

            //simcard检测
            if (!MonitorDataUtil.HasSimCard())
            {
                if (SwitchDevice && SwitchDeviceCard)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.sim);
                sim = 0;
            }

            //模拟器检测
            bool isSimulator = false;
            if (MonitorDataUtil.IsEmulator() || MonitorDataUtil.IsEmulator2())
            {
                isSimulator = true;
            }

            if (isSimulator)
            {
                if (SwitchDevice && SwitchDeviceSimulator)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.simulator);
                simulator = 1;
            }

            //root权限检测
            if (MonitorDataUtil.IsXposed())
            {
                if (SwitchDevice && SwitchDeviceRootPower)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.root);
                root = 1;
            }

            //开发者模式检测
            if (MonitorDataUtil.IsDebug())
            {
                if (SwitchDevice && SwitchDeviceDevelopeMode)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.developer);
                developer = 1;
            }


            //由于用户行为触发风控的将会被记录,后续登录仍会处于风控状态
            bool shortClose = PlayerPrefs.GetInt(MonitorTrigger.trigger_short_close.ToString()) == 1;
            bool shortShow = PlayerPrefs.GetInt(MonitorTrigger.trigger_short_show.ToString()) == 1;
            bool adFirst = PlayerPrefs.GetInt(MonitorTrigger.trigger_first_ad.ToString()) == 1;
            bool deemMore = PlayerPrefs.GetInt(MonitorTrigger.trigger_deem_more.ToString()) == 1;
            bool deemLess = PlayerPrefs.GetInt(MonitorTrigger.trigger_deem_less.ToString()) == 1;
            if ((shortClose && SwitchAdvInterval) || (shortShow && SwitchAdvInterval) || (adFirst && SwitchFirstAdv) ||
                (deemMore && SwitchDeemAdv) || (deemLess && SwitchDeemAdv))
            {
                if (SwitchBehavior)
                {
                    RvLimited = true;
                    IVLimited = true;
                }
            }

            //初始化各类计数器，并检测广告次数是否达到限制
            RVCounter = PlayerPrefs.GetInt(MonitorAssetsEnum.assets_rv_counter.ToString());
            IVCounter = PlayerPrefs.GetInt(MonitorAssetsEnum.assets_iv_counter.ToString());
            ShortShowCounter = PlayerPrefs.GetInt(MonitorAssetsEnum.shortshow_counter.ToString());
            ShortCloseCounter = PlayerPrefs.GetInt(MonitorAssetsEnum.shortclose_Counter.ToString());
            int yesterday = PlayerPrefs.GetInt(MonitorAssetsEnum.new_day.ToString());
            int today = DateTime.UtcNow.DayOfYear;
            if (today > yesterday || (today == 1 && yesterday > 360))
            {
                RVCounter = 0;
                IVCounter = 0;
            }

            if (RVCounter >= 
                RvLimitCount)
            {
                RvLimited = true;
            }

            if (IVCounter >= IvLimitCount)
            {
                IVLimited = true;
            }


            //发送session_custom事件
            StatisticsManager.Instance.SendLogEvent(MonitorEventEnum.session_custom.ToString(),
                new Dictionary<string, object>
                {
                    { RiskFromEnum.vpn.ToString(), vpn },
                    { RiskFromEnum.root.ToString(), root },
                    { RiskFromEnum.simulator.ToString(), simulator },
                    { RiskFromEnum.sim.ToString(), sim },
                    { RiskFromEnum.developer.ToString(), developer },
                    { RiskFromEnum.googleplay.ToString(), google },
                    {
                        MonitorEventEnum.user_source.ToString(), MonitorDataUtil.GetInstallSource()
                    }, //user_source 用户来源 sdk里获取 自己接Appsflyer的话则从appsflyer相关接口获取
                    {
                        MonitorEventEnum.install_source.ToString(), AdjustManager.Instance.mediaSource
                    } //install_source  获取安装源 sdk里获取
                });


            PlayerPrefs.SetInt(MonitorAssetsEnum.new_day.ToString(), today);
            Debug.Log(
                $"Country:{MonitorDataUtil.GetCountry()} --- Language:{MonitorDataUtil.GetLanguage()} --- Emulator:{MonitorDataUtil.IsEmulator()} --- Emulator2:{MonitorDataUtil.IsEmulator2()} --- DevModel:{MonitorDataUtil.IsDevModel()} --- DebugL:{MonitorDataUtil.IsDebug()} --- Xposed:{MonitorDataUtil.IsXposed()} --- AbnormalEnv:{MonitorDataUtil.IsAbnormalEnv()} --- Vpn:{MonitorDataUtil.IsVpn()} --- Proxy:{MonitorDataUtil.IsProxy()} --- Sim:{MonitorDataUtil.HasSimCard()} --- SimMcc:{MonitorDataUtil.GetSimMcc()} --- InstallSource:{MonitorDataUtil.GetInstallSource()}");
        }

        //开始播放广告时调用(激励广告)
        public static void OnPlayVideoStart()
        {
            startAdTime = DateTime.UtcNow.Ticks / 10000;
        }

        /// <summary>
        /// 播放广告结束调用
        /// </summary>
        /// <param name="type"> 0:rv,  1:iv </param>
        /// <param name="source"> 广告回调参数 adInfo.NetworkName </param>
        public static void OnPlayVideoEnd(int type, string source)
        {
            RVCounter += type == 0 ? 1 : 0;
            IVCounter += type == 1 ? 1 : 0;
            if (RVCounter >= RvLimitCount)
            {
                RvLimited = true;
                //发送事件 广告数量超限事件
                StatisticsManager.Instance.SendLogEvent(RiskFromEnum.see_you_tommorrow0.ToString());
            }

            if (IVCounter >= IvLimitCount)
            {
                IVLimited = true;
                //发送事件 广告数量超限事件
                StatisticsManager.Instance.SendLogEvent(RiskFromEnum.see_you_tommorrow1.ToString());
            }

            if (type == 0)
            {
                //风控Short_Show
                long time = DateTime.UtcNow.Ticks / 10000; //毫秒
                if (lastAdTime > 0)
                {
                    int second = Convert.ToInt32((time - lastAdTime) / 1000);
                    ShortShowCounter += second < 30 ? 1 : 0;
                    if (ShortShowCounter >= 3)
                    {
                        if (SwitchBehavior && SwitchAdvInterval)
                        {
                            RvLimited = true;
                            IVLimited = true;
                        }

                        TrackRiskChance(RiskFromEnum.ad_short_show);
                        PlayerPrefs.SetInt(MonitorTrigger.trigger_short_show.ToString(), 1);
                    }
                }

                //风控Short_Close
                if (startAdTime > 0)
                {
                    int second = Convert.ToInt32((time - startAdTime) / 1000);
                    ShortCloseCounter += second < 20 ? 1 : 0;
                    if (ShortCloseCounter >= 3)
                    {
                        if (SwitchBehavior && SwitchAdvInterval)
                        {
                            RvLimited = true;
                            IVLimited = true;
                        }

                        TrackRiskChance(RiskFromEnum.ad_short_close);
                        StatisticsManager.Instance.SendLogEvent(RiskFromEnum.ad_short_close.ToString(),
                            new Dictionary<string, object> { { MonitorEventEnum.ad_source.ToString(), source } });
                        PlayerPrefs.SetInt(MonitorTrigger.trigger_short_close.ToString(), 1);
                    }
                }

                //风控First_Ad
                if (!GameCommon.PlayerFirstOpenApp)
                {
                    if (SwitchBehavior && SwitchFirstAdv)
                    {
                        RvLimited = true;
                        IVLimited = true;
                    }

                    TrackRiskChance(RiskFromEnum.first_rv_time);
                    PlayerPrefs.SetInt(MonitorTrigger.trigger_first_ad.ToString(), 1);
                }

                //风控Deem_More
                if (RVCounter > DeemMoreCount && !hasAnyTaskProgressing) //判断是否有任何提现任务已经开始 逻辑自己写
                {
                    if (SwitchBehavior && SwitchDeemAdv)
                    {
                        RvLimited = true;
                        IVLimited = true;
                    }

                    TrackRiskChance(RiskFromEnum.wrong_deem_ad_more);
                    PlayerPrefs.SetInt(MonitorTrigger.trigger_deem_more.ToString(), 1);
                }

                lastAdTime = time;
            }
        }

        //提现任务开始时调用
        public static void OnDeemStart()
        {
            //风控Deem_Less
            if (RVCounter < DeemLessCount)
            {
                if (SwitchBehavior && SwitchDeemAdv)
                {
                    RvLimited = true;
                    IVLimited = true;
                }

                TrackRiskChance(RiskFromEnum.wrong_deem_ad_less);
                PlayerPrefs.SetInt(MonitorTrigger.trigger_deem_less.ToString(), 1);
            }
        }

        public static void TrackRiskChance(RiskFromEnum from)
        {
            //tba 发送riskchance事件
            StatisticsManager.Instance.SendLogEvent(MonitorEventEnum.risk_chance.ToString(),
                new Dictionary<string, object> { { MonitorEventEnum.rish_from.ToString(), from.ToString() } });
        }
    }
}

public enum RiskFromEnum
{
    ad_short_close = 0,
    ad_short_show = 1,
    first_rv_time = 2,
    wrong_deem_ad_less = 3,
    wrong_deem_ad_more = 4,
    vpn = 5,
    root = 6,
    sim = 7,
    simulator = 8,
    developer = 9,
    googleplay = 10,
    see_you_tommorrow0 = 11,
    see_you_tommorrow1 = 12,
    number = 13  //数字联盟
}

public enum MonitorEventEnum
{
    risk_chance,
    rish_from,
    session_custom,
    user_source,
    install_source,
    ad_source
}

public enum MonitorAssetsEnum
{
    assets_rv_counter = 0,
    assets_iv_counter = 1,
    shortshow_counter = 2,
    shortclose_Counter = 3,
    new_day
}    
public enum MonitorTrigger
{
    trigger_short_show,
    trigger_short_close,
    trigger_first_ad,
    trigger_deem_more,
    trigger_deem_less
}