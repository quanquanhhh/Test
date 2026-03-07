namespace Foundation
{
    [System.Serializable]
    public class APPConfig
    {
        public string platform;
        public string game_name;
        public string package_name;
        public string email;
        public string firebase_topic;
        public Libs libs;
    }
    [System.Serializable]
    public class Libs
    {
        public AppsFlyers appsflyer;
        public Adjusts_Config adjust;
        public string fb_appid;
        public string fb_apptoken;
        public Max_config max;
        public Topon topon;
    }
    
    [System.Serializable]
    public class AppsFlyers
    {
        public string dev_key;
        public string ios_appid;
    }
    
    [System.Serializable]
    public class Adjusts_Config
    {
        public string dev_key;
    }

    [System.Serializable]
    public class Max_config
    {
        public string rewarded_ad_unit_id;
        public string interstitial_ad_unit_id;
    }

    [System.Serializable]
    public class Topon
    {
        public string app_id;
        public string app_key;
        public string rewarded_ad_unit_id;
        public string interstitial_ad_unit_id;
    }
    
    
}