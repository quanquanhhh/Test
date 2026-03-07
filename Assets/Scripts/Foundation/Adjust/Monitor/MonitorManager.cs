using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public class MonitorData
    {
        public List<int> Switch = new List<int>();
        public List<int> BehaviorSwitch = new List<int>();
        public List<int> DeviceSwitch = new List<int>();
        public List<int> DeemLess = new List<int>();
        public List<int> RvLimit = new List<int>();
        public List<int> IvLimit = new List<int>();
        public List<int> IsShaving = new List<int>();
        
    }
    public class MonitorManager : SingletonComponent<MonitorManager>
    {
        public static bool IsInitMonitor = false;
        private static MonitorData  _monitorData = new MonitorData();  
        public void InitMonitorData(MonitorData data)
        {
            _monitorData = data;
             // var data = ConfigInfo.Monitor;  //JsonDataMgr.MonitorSData;
             MonitorUtil.SwitchNumber = data.Switch[0] == 1 ? true : false;
             MonitorUtil.SwitchBehavior = data.Switch[1] == 1 ? true : false;
             MonitorUtil.SwitchDevice = data.Switch[2] == 1 ? true : false;

             MonitorUtil.SwitchAdvInterval = data.BehaviorSwitch[0] == 1 ? true : false;
             MonitorUtil.SwitchFirstAdv = data.BehaviorSwitch[1] == 1 ? true : false;
             MonitorUtil.SwitchDeemAdv = data.BehaviorSwitch[2] == 1 ? true : false;

             MonitorUtil.SwitchDeviceVpn = data.DeviceSwitch[0] == 1 ? true : false;
             MonitorUtil.SwitchDeviceRootPower = data.DeviceSwitch[1] == 1 ? true : false;
             MonitorUtil.SwitchDeviceCard = data.DeviceSwitch[2] == 1 ? true : false;
             MonitorUtil.SwitchDeviceSimulator = data.DeviceSwitch[3] == 1 ? true : false;
             MonitorUtil.SwitchDeviceDevelopeMode = data.DeviceSwitch[4] == 1 ? true : false;
             MonitorUtil.SwitchDeviceGoogle = data.DeviceSwitch[5] == 1 ? true : false;

             MonitorUtil.DeemLessCount = data.DeemLess[0];
             MonitorUtil.DeemMoreCount = data.DeemLess[0];

             MonitorUtil.RvLimitCount = data.RvLimit[0];
             MonitorUtil.IvLimitCount = data.IvLimit[0];
             IsInitMonitor  =true;
         }
         public void InitMonitor()
         {
              MonitorUtil.StartMonitorControl();

              CheckUserState();
         }
         public void CheckUserState()
         {
             if (MonitorUtil.RvLimited && MonitorUtil.IVLimited && _monitorData.IsShaving[0] == 1)
             {
                 Debug.LogError("LeGeLook  AF NOT iNIT");
             }
             else
             {
                 Debug.LogError("LeGeLook  AF go to Shaving");
                 ShavingManager.Instance.OnInitialization();
             }
         }

        
    }
}