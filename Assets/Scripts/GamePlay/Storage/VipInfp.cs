using System;
using Foundation.Storage;
using Newtonsoft.Json;

namespace GamePlay.Storage
{
    [Serializable]
    public class VipInfp
    {
        [JsonProperty] private bool isVip = false;
        [JsonIgnore]
        public bool IsVip
        {
            get { return isVip; }
            set
            {
                isVip = value;
                StorageManager.Instance.ForceSave = true;
            }
        }
        [JsonProperty] private ulong vipExpire ;
        [JsonIgnore]
        public ulong IsVipExpire
        {
            get { return vipExpire; }
            set
            {
                vipExpire = value;
                StorageManager.Instance.ForceSave = true;
            }
        }
        
        
    }
}