using System;
using Foundation.FSM;
using UnityEngine;

namespace GamePlay.LauncherFsm
{
    public class LauncherSetUpView : LauncherBase
    {
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 0.001f;
            base.OnInit(fsm);
        }

        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        {
            base.OnEnter(fsm);
            
            ViewUtility.SetUpViewSize();
            var rootGameObject = GameObject.Find("Root");
            GameObject.DontDestroyOnLoad(rootGameObject);
            ChangeState<LauncherShowLoading>(fsm);
        }
    }
}