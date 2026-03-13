using Foundation;
using GamePlay.Component;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.UIMain
{
    [Window("MainUI",WindowLayer.System)]
    public class MainUI : UIWindow
    {
        [UIBinder("Btn", "Vip")] private Button vipEntrance;

        public override void OnCreate()
        {
            base.OnCreate();
            vipEntrance.onClick.AddListener(OpenBuyVipPop);

            var test = new GameObject();
            test.name = "Test";
            test.transform.parent = rectTransform;
            test.transform.localScale = Vector3.one;
            test.transform.localPosition = Vector3.zero;
            var testcomponent = test.AddComponent<UguiMediaSource>();
            testcomponent.SetSource("Ashipinjpg_AMP4001", false);
        }

        private void OpenBuyVipPop()
        {
            UIModule.Instance.ShowAsync<BuyVipPop>();
        }
    }
}