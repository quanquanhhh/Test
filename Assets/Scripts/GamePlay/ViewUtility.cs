using System;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace GamePlay
{
    public static class ViewUtility
    {
        public static Vector2 DesignSize = new Vector2(1080, 1920);
        public static Vector2 referenceResolutionPortrait;
        public static void SetUpViewSize()
        {
            var screenWidth = Math.Max(Screen.width, Screen.height);
            var screenHeight = Math.Min(Screen.width, Screen.height);

            var referenceSizeY = DesignSize.y;
            var referenceWidth = (float) (screenWidth) / screenHeight * referenceSizeY;
 
            referenceResolutionPortrait = new Vector2(referenceSizeY, referenceWidth);
        }
    }
}