using System;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace GamePlay
{
    public class ViewUtility
    {
        public static Vector2 designSize = new Vector2(1920, 1080);
        public static Vector2 referenceResolutionPortrait;
        public static void SetUpViewSize()
        {
            var screenWidth = Math.Max(Screen.width, Screen.height);
            var screenHeight = Math.Min(Screen.width, Screen.height);

            var referenceSizeY = designSize.y;
            var referenceWidth = (float) (screenWidth) / screenHeight * referenceSizeY;
 
            referenceResolutionPortrait = new Vector2(referenceSizeY, referenceWidth);
        }
    }
}