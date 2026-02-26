using Foutain.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件接口（所有事件都要实现这个）
public interface IGameEvent { }
// ==================== 如何新增事件 ====================
//[System.Serializable]
//public class 事件类的名字 : IGameEvent
//{
//    public string CustomerName;      // 传递参数一
//    public string Order;            // 传递参数二
//    public int PatienceTime;        // 传递参数三
//}

// ==================== 游戏暂停面板事件 ====================
[System.Serializable]
public class MenuEvent : IGameEvent
{
    
}

[System.Serializable]
public class SettingEvent : IGameEvent
{
    
}

[System.Serializable]
public class ContinueEvent : IGameEvent
{
    
}

[System.Serializable]
public class GamePauseEvent:IGameEvent
{

}

public class LocaleChangeEvent:IGameEvent
{
    public LocalizationManager.LocaleID locale;
}
