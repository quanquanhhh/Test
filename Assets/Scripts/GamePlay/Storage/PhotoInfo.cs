using System.Collections.Generic;
using Foundation.Storage;
using Newtonsoft.Json;

namespace GamePlay.Storage
{
    enum PhotoStae
    {
        None = 0,
        Lock = 1,
        Unlock = 2,
        Remove = 3
    }
    [System.Serializable]
    public class PhotoInfo : StorageBase
    {
        [JsonProperty] private Dictionary<string, int> photoState;
        [JsonIgnore]
        public Dictionary<string, int> PhotoState => photoState;
        
    }
}