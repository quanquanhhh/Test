// using Foundation;
// using Unity.VisualScripting;
// using YooAsset;
//
// namespace GamePlay.LauncherFsm
// {
//     public class PatchOperation : GameAsyncOperation
//     {
//         
//         private enum ESteps
//         {
//             None,
//             Update,
//             Done,
//         }
//         private LauncherFsm _machine;
//         private string _packageName;
//         private ESteps _steps = ESteps.None;
//
//         public PatchOperation(string packageName, EPlayMode playMode)
//         {
//             _packageName = packageName;
//             Event.Instance.Subscribe<UserTryInitialize>(OnUserTryInitialize);
//             Event.Instance.Subscribe<UserBeginDownloadWebFiles>(OnUserBeginDownloadWebFiles);
//             Event.Instance.Subscribe<UserTryRequestPackageVersion>(OnUserTryRequestPackageVersion);
//             Event.Instance.Subscribe<UserTryUpdatePackageManifest>(OnUserTryUpdatePackageManifest);
//             Event.Instance.Subscribe<UserTryDownloadWebFiles>(OnUserTryDownloadWebFiles);
//             
//
//             // 创建状态机
//             _machine = new LauncherFsm(); 
//             
//             _machine.Initialize();
//         }
//         protected override void OnStart()
//         {
//             _steps = ESteps.Update;
//             _machine.Start();
//         }
//         protected override void OnUpdate()
//         {
//             if (_steps == ESteps.None || _steps == ESteps.Done)
//                 return;
//
//             if (_steps == ESteps.Update)
//             {
//                 _machine.Update();
//             }
//         }
//         protected override void OnAbort()
//         {
//         }
//
//         public void SetFinish()
//         {
//             _steps = ESteps.Done;
//             _eventGroup.RemoveAllListener();
//             Status = EOperationStatus.Succeed;
//             Debug.Log($"Package {_packageName} patch done !");
//         }
//
//         /// <summary>
//         /// 接收事件
//         /// </summary>
//         private void OnHandleEventMessage(IEventMessage message)
//         {
//             if (message is UserEventDefine.UserTryInitialize)
//             {
//                 _machine.ChangeState<FsmInitializePackage>();
//             }
//             else if (message is UserEventDefine.UserBeginDownloadWebFiles)
//             {
//                 _machine.ChangeState<FsmDownloadPackageFiles>();
//             }
//             else if (message is UserEventDefine.UserTryRequestPackageVersion)
//             {
//                 _machine.ChangeState<FsmRequestPackageVersion>();
//             }
//             else if (message is UserEventDefine.UserTryUpdatePackageManifest)
//             {
//                 _machine.ChangeState<FsmUpdatePackageManifest>();
//             }
//             else if (message is UserEventDefine.UserTryDownloadWebFiles)
//             {
//                 _machine.ChangeState<FsmCreateDownloader>();
//             }
//             else
//             {
//                 throw new System.NotImplementedException($"{message.GetType()}");
//             }
//         }
//     }
// }