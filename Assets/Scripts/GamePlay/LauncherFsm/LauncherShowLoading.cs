using Foundation.FSM;
using UnityEngine;

namespace GamePlay.LauncherFsm
{
    public class LauncherShowLoading : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 0.0001f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            
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