using System;
using Foundation;
using Foundation.FSM;
using UnityEngine;

namespace GamePlay.LauncherFsm
{
    public class LauncherSetUpView : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 2f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
         
            
            ViewUtility.SetUpViewSize();
            var rootGameObject = GameObject.Find("Root");
            GameObject.DontDestroyOnLoad(rootGameObject);
            
            var mgr = GameObject.Find("Mgr");
            GameObject.DontDestroyOnLoad(mgr);
            GameViewComponent._loadingLuncher = mgr.GetComponent<LoadingLuncher>();
            RootManager.MgrRoot = mgr;
            RootManager.UIRoot = GameObject.Find("Root/UIRoot/UICanvas").transform;
            
            ShowLoading(fsm); 
        }
        private void ShowLoading(IFsm<LauncherFsm> fsm)
        {
            var load = Resources.Load<GameObject>("Loading/Loading");
            var uiCanvas = GameObject.Find("Root/UIRoot/UICanvas");
            if (uiCanvas != null)
            {
                var loadView = GameObject.Instantiate(load, uiCanvas.transform);
                loadView.name = "Loading";
                fsm.Owner.LoadingView = loadView;
            }
            
            ChangeState<LauncherGetResourceVersion>(fsm);
        }
    }
}