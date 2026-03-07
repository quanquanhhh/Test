using System;
using UnityEngine;

namespace Foundation
{
    public interface IEvent
    {
    } 
    public struct UpLevelEvent : IEvent { }
    public struct CheckAccountFinished : IEvent { }
    public struct IntoGame : IEvent { }
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
    
    public struct GetReward : IEvent
    {
        public Vector3 pos;
        public int amount;
        public int rewardType;
        public GetReward( int rewardType, int amount,Vector3 pos)
        {
            this.pos = pos;
            this.amount = amount;
            this.rewardType = rewardType;
        }
    }
}