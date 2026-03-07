using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foundation
{
    public class UniTaskMgr : SingletonComponent<UniTaskMgr>
    {
         
        
        public async UniTask WaitForSecond(float second)
        {
            await UniTask.Delay((int)(second * 1000), cancellationToken: SingleObj.GetCancellationTokenOnDestroy());
        }

        public async UniTask WaitForSecond(float second, Action callback)
        {
            await UniTask.Delay((int)(second * 1000), cancellationToken: SingleObj.GetCancellationTokenOnDestroy());
            callback?.Invoke();
        }

        public async UniTask WaitForSecond(float second, Action callback, CancellationToken token)
        { 
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                SingleObj.GetCancellationTokenOnDestroy()
            );

            try
            {
                await UniTask.Delay((int)(second * 1000), cancellationToken: linkedCts.Token);
                callback?.Invoke();   

            }
            catch (OperationCanceledException e)
            { 
            } 
        }

        public async UniTask Delay(int frame)
        {
            await UniTask.Delay(frame, cancellationToken: SingleObj.GetCancellationTokenOnDestroy());
        }

        public async UniTask Yield()
        {
            await UniTask.Yield(cancellationToken: SingleObj.GetCancellationTokenOnDestroy());
        }
        public void OnDestroy()
        {
            if (SingleObj != null)
            {
                GameObject.Destroy(SingleObj);
            }
        }
    }
}