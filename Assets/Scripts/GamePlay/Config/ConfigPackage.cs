using System;
using System.Collections.Generic;
using Foundation.Storage;
using GamePlay.Storage;
using Unity.VisualScripting;

namespace GameConfig
{
    enum PhotoType
    {
        Normal = 0,
        Ad = 1,
        Sign = 2,
        NewGift = 3,
        Shop = 4,
        Select = 5
    }
    [Serializable]
    public class ConfigPackage
    {
        public List<BaseGame> BaseGame;
        public List<Shop> Shop;
        public List<Item> Item;
        public List<Photo> Photo;
        public List<Sign> Sign;
        public List<Difficult> Difficult;
        public List<DifficultConfig> DifficultConfig;
    }

    public static class GameConfig
    {
        
        public static void InitConfig(ConfigPackage config)
        {
            baseGame = config.BaseGame[0];
            shop = config.Shop;
            item = config.Item;
            // photo = config.Photo;
            sign = config.Sign;
            leveldifficult =  config.Difficult;
            difficultConfig = config.DifficultConfig;
            DealPhotos(config.Photo);
        }
        
        public static BaseGame baseGame;
        public static List<Shop> shop;
        public static List<Item> item;
        // public static List<Photo> photo;
        public static List<Sign> sign;
        public static List<Difficult> leveldifficult;
        public static List<DifficultConfig> difficultConfig;
        
        public static List<Photo> HighLevelPhotos = new  List<Photo>();
        public static List<Photo> AdPhotos = new List<Photo>();
        public static List<Photo> NormalPhotos = new List<Photo>();

        private static bool VaildPhoto(string name)
        {
            var data = StorageManager.Instance.GetStorage<PhotoInfo>();
            if (!data.PhotoState.ContainsKey(name))
            {
                return true;
            }
            return data.PhotoState[name] != (int)PhotoStae.Remove;
        }
        public static void DealPhotos(List<Photo> config)
        {
            foreach (var photo in config)
            {
                if (!VaildPhoto(photo.name))
                {
                    continue;
                }
                if (photo.level == 1)
                {
                    HighLevelPhotos.Add(photo);
                }
                else if (photo.sourceFrom == (int)PhotoType.Normal)
                {
                    NormalPhotos.Add(photo);
                }
                else if (photo.sourceFrom == (int)PhotoType.Ad)
                {
                    AdPhotos.Add(photo);
                }
            }
        }
    }
}