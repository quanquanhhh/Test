using Foundation;
using Foundation.FSM;
using UnityEngine;

namespace GamePlay.LauncherFsm
{
    public class LauncherGame : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 0.01f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            var a =AssetLoad.Instance.LoadGameObjectSync("TestPrefab");
            Debug.Log( a == null ? " 1 " :"2");
        }
    }
}