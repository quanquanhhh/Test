using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;

namespace Foundation
{
    public class AssetLoad : SingletonComponent<AssetLoad>
    {
        private Dictionary<string, SpriteAtlas> _atlasMap = new Dictionary<string, SpriteAtlas>();
        public T LoadAssetSync<T>(string location) where T : Object
        {
            if (string.IsNullOrEmpty(location))
            {
                return null;
            } 
            AssetHandle handle = YooAssets.LoadAssetSync<T>(location);
            T asset = handle.AssetObject as T; 
            return asset;
        }
        
        public GameObject LoadGameObjectSync(string name,Transform parent = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var obj = LoadAssetSync<GameObject>(name);
            if (obj == null)
            {
                return null;
            }
            var objInstant = GameObject.Instantiate(obj, parent);
            return objInstant;
        }
        public async UniTask<GameObject> LoadGameobjectAsync(string assetName, Transform parent = null)
        {
            var handle = LoadAssetAsync<GameObject>(assetName);
            
            await handle.Task;
            if (handle.AssetObject != null)
            {
                GameObject obj = GameObject.Instantiate(handle.AssetObject as GameObject, parent);
                return obj;
            }
            Debug.Log(" not asset ");
            return null;
        }
        
        public async UniTask<T> LoadAsset<T>(string assetName) where T : Object
        {
            var handle = LoadAssetAsync<T>(assetName);
            await handle.Task;
            return handle.AssetObject as T;
        } 
        public Sprite LoadSprite(string assetName, string atlasName = "CommonAtlas")
        {
            if (!_atlasMap.ContainsKey(atlasName))
            {
                var atlas = LoadAssetSync<SpriteAtlas>(atlasName);
                _atlasMap.Add(atlasName, atlas);
            }

            return _atlasMap[atlasName].GetSprite(assetName);
        }
        private static AssetHandle LoadAssetAsync<T>(string location) where T : Object
        {
            return YooAssets.LoadAssetAsync<T>(location);
        }
        
    }
}