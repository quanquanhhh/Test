using Foundation.FSM;

namespace GamePlay.LauncherFsm
{
    public class LauncherBase : FsmState<LauncherFsm>
    {
        public  float stepDeltaProgress = 0.02f;
         
        protected internal override void OnInit(IFsm<LauncherFsm> fsm)
        {
            stepDeltaProgress = 0.02f;
            base.OnInit(fsm);
        }
        protected internal override void OnEnter(IFsm<LauncherFsm> fsm)
        { 
            base.OnEnter(fsm);

            fsm.Owner.accumulateProgress += stepDeltaProgress;
        }

    }
}