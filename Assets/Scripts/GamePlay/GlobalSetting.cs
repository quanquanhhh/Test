using UnityEngine;

namespace GamePlay
{
    public class GlobalSetting
    {
        
        public static string PackageName => "RH4";
        protected static string resourcesVersion = "v1";
        
        public static string ResourceVersion
        {
            get { return resourcesVersion; }
            set { resourcesVersion = value; }
        }
        

        public static string GetHostResUrl()
        {
            string url = $"https://bunny.sheriffbunny.com/Test/{Application.version}/{resourcesVersion}";
            return url;
        }
    }
}