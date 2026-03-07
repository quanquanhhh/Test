using Newtonsoft.Json;
using UnityEngine;

namespace Foundation
{
    public class ConfigSelect
    {

        private string ConfigTxtPath = "/Resources/Config/";
        private const string RELEASE_CONFIG_PATH = "Config/config";
        private const string ENCRYPTION_KEY = "YourEncryptionKey123";
        
        public static string IOSTest =
            "ImJ/UmVMEx4YBBIGHQNpX1kTe1wqTVl/T05DUB4RGQwwACoIHBMIE3s9ABwrBw0VJiQRGhsxCS9bHT85eU9XAiQNCBMeFSsHDgMuR0MREFA2AlsQIAcJGxcXWgsKByEMF1YcRzwcAVBpY2lSWVIRBA4HJ0dDERARdWJ/UmVMBRsLFRYIHAsUERZBW1B7VVVQZ0JueFlQVgUGDDhHQxFJPlNPVVJlTAICCQMSBRYLOUdDEUkRPQoDLS4LGlBDUFaO+8atyNrUjry+9fFQaU5BGxYDKwgfHiIBWwsSEWFdRERzXFJKQUhWFENjQUVZERIROAsfBzYaQUhZC1YNChgUDhxIEAl5TRhAKwsKBBIaRV8ZWmkYVTw4E3lPVVAjDDwTCQAdDU1Ua0dbHT85eU9VUmcIAS0YAAQdAAUuC1sLEhF7Q3h4ZU5DUlsdFRFNVGsedDsSE3lPVVJnHAYFGAIQDAsxKgEmRFxaLTAcFmdUQUVNEhZaWw8vA0sFCwdhDkNQaWNpUllQVElPTCILDVRAQC0GARskAjwTHS8BBwYaFAwdEwgRaA1FRHNdVBRNRhVZXQ98Bls8OBN5T1UPaWNpUllQVEsbATsKFxMIEyJif1JlTkNSWVIVGR8xIgFbCxIRe0N4eGVOQ1JZUFYIHx4UDhxIEAl5TVdeSGRDUllQVElNHC4SGENWVj0wFBYaGw0bDS8dDU1UaUdVPDgTeU9VUmVMChwNFQYaGwc/DBhdbVI9MAAcLBo8Gx1STktNY0FFWRESTlRlVVI4Y2kP";

        public static string AndroidTest =
            "ImJ/UmVMEx4YBBIGHQNpX1kTc109HRobIUxPf3NQVEsIDyYAJl9TXjxNT1JnPBYcFxkaDjA6LhYNEx4+U09VUDUPABkYFxE2AQ8mAFsLEhE6ABhcMQsQBlcUEQQAQCoVCRMePlNPVVAgAwIbFVJOSU0XPgkccUZWKhsXGzcKTREWHVZFYmRrRVtXW0E8DRQBIDEXHQkZF0tVTmkXDF9cWjcIKgYgHRdQVX1+SU9MJwwbQhAJeRR4eGVOQ1JbEQQZHAgnHBxDEAl5FFcWIBg8GRwJVlNPTDwIKUZ5Xg8pQSc9Jg0oPiQ3DiwBcwdbHRIRMAAGLSQeExsdUk5JTUw2SXQ7EhN5T1cTIQQWAQ1STkkUTC8AD25ZViBNT1JnCQsBTQEBHRYEOQxBE08fVGVVUmVOQRQbLxUZHwcvR0MREBF1Yn9SZU5DUB8SKwgfHj8KElRcEWNPV1BpY2lSWVBUSwIPM0dDEUk+U09VUmVOQ1ALFQMIHQouASZQVmwsARwGGgcHUENSEV5cD3hUSQkABmsJRUZ1VkFedHpUSU9Oa0VbWFxHPB0GBiwaChMVLxUNMBslDA1uW1d7VVcTc1YFRxsSFlBXDXkHHVIGEVRlVVJlTh5edHpUSU9OaREWQV1de1VVCUhkQ1JZUFRJTQ87FSZYVhFjT1cTc1lVF00VFQxbXykBGBMePlNPVVJlTkNQGAAENgQLMkdDERBSYApARXwIBhMdFhJbCl19XE4FAlY4WxAXcl0FFh0RQ19NQkZvWRESE3lPVwAgGQIAHRUQNg4KFBAXWEZsMAtXSGcMVUVPFUAMClopVxgDChF1Yn9SZU5DUllSHQcbCzkWDVhGWjgDKhMhMRYcEAQrAAtMcUcbBwUFPFsQFyMLVhEaFFZkZU5rRVlMPzl5Twh/TxM=";

        private static string useKey;

        public static APPConfig useConfig;

        public static string AdjustDevKey
        {
            get
            {
                return useConfig.libs.adjust.dev_key;
            }
        }

        public static string AdRewardUnit
        {
            get
            {
                return useConfig.libs.max.rewarded_ad_unit_id;
            }
        }

        public static string AdInterstitialUnit
        {
            get
            {
                return useConfig.libs.max.interstitial_ad_unit_id;
            }
        }

        
        
        public static void GetUseConfig()
        { 
#if UNITY_IOS && (DEBUG_APP || DEBUG) 
            useKey = ConfigSelect.IOSTest;
#elif UNITY_ANDROID && (DEBUG_APP || DEBUG)
            useKey = ConfigSelect.AndroidTest;
#elif UNITY_EDITOR
            useKey = ConfigSelect.AndroidTest;
#elif UNITY_IOS && !DEBUG
            TextAsset textAsset = Resources.Load<TextAsset>(RELEASE_CONFIG_PATH);
            if (textAsset != null) useKey = textAsset.text;
#endif
            if (string.IsNullOrEmpty(useKey))
            {
                useKey = AndroidTest;
            }
            
            useConfig = LoadConfig<APPConfig>(useKey);
        }
        private static T LoadConfig<T>(string path) where T : class
        {
            try
            {
                string encryptedContent = path;
            
                string jsonContent = Decrypt(encryptedContent, ENCRYPTION_KEY);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    jsonContent = encryptedContent;
                }
                T config = JsonConvert.DeserializeObject<T>(jsonContent); 
                return config;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }
        
        private static string Decrypt(string encryptedText, string key)
        {
            if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(key))
                return encryptedText;
            
            try
            {
                byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
                byte[] decryptedBytes = new byte[encryptedBytes.Length];
            
                for (int i = 0; i < encryptedBytes.Length; i++)
                {
                    decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
                }
            
                return System.Text.Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return null;
            }
        } 
        
        
    }
}