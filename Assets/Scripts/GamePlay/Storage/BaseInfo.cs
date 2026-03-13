using Foundation.Storage;
using Newtonsoft.Json;

namespace GamePlay.Storage
{
    [System.Serializable]
    public class BaseInfo : StorageBase
    {
        [JsonProperty] private int level = 1;

        [JsonIgnore]
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                StorageManager.Instance.ForceSave = true;
            }
        }
        
        [JsonProperty] private VipInfp vip ;
        [JsonIgnore]
        public VipInfp Vip
        {
            get { return vip; }
        }



    }
}