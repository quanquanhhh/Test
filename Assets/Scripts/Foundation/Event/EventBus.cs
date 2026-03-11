using System;
using UnityEngine;

namespace Foundation
{
    public interface IEvent
    {
    } 
     
    public struct EventTask : IEvent
    {
        public int taskType;
        public int count;
        public EventTask(int taskType, int count = 0)
        {
            this.taskType = taskType;
            this.count = count;
        }
    }
    public struct ShowSystemTips : IEvent
    {
        public string text;
        public bool isDebug;
        public ShowSystemTips(string text, bool isDebug = false)
        {
            this.text = text;
            this.isDebug = isDebug;
        }
    }
    public struct RVPlayFinished : IEvent
    {
        public Action action; 
        public RVPlayFinished(Action a )
        {
            action = a; 
        }
    }
    
    public struct IVPlayFinished : IEvent
    {
        public Action action; 
        public IVPlayFinished(Action a )
        {
            action = a; 
        }
    }
    public struct UserTryInitialize : IEvent { }
    public struct UserBeginDownloadWebFiles : IEvent { }
    public struct UserTryRequestPackageVersion : IEvent { }
    public struct UserTryUpdatePackageManifest : IEvent { }
    public struct UserTryDownloadWebFiles : IEvent { }
     
}