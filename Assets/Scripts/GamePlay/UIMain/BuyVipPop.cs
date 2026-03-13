using Foundation;
using UnityEngine.UI;

namespace GamePlay.UIMain
{
    [Window("BuyVipPop", WindowLayer.Popup)]
    public class BuyVipPop : UIWindow
    {
        [UIBinder("CloseBtn")] private Button closeBtn;
        [UIBinder("BuyBtn")] private Button buyBtn;
        [UIBinder("Privacy")] private Button PrivacyBtn;
        [UIBinder("Term")] private Button TermBtn;
        
        
        public override void OnCreate()
        {
            base.OnCreate();
            closeBtn.onClick.AddListener(Close);
            buyBtn.onClick.AddListener(BuyVip);
        }

        private void BuyVip()
        {
            BuyUtility.OpenPurcher("",BuySuccess);
            
        }

        private void BuySuccess()
        {
            
        }
    }
}