using Fountain.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

[System.Serializable]
public class LocaleChangeEvent:IGameEvent
{
    public LocalizationManager.LocaleID locale;
}

[System.Serializable]
public class DialogueBeginEvent:IGameEvent
{

}
[System.Serializable]
public class DialogueEndEvent:IGameEvent
{

}

[System.Serializable]
public class SettingBackEvent : IGameEvent { }

[System.Serializable]
public class GameStartEvent : IGameEvent { }

// ==================== 脚本触发事件 ====================
[System.Serializable]
public class ScriptTriggerEvent : IGameEvent
{
    public string TriggerId; // 目标触发器标识符（留空则触发所有ScriptTrigger）
    public string DialogueID; // 可选：关联的对话ID（如果需要）
}

//下面几个是控制喷泉玩法需要的事件
[System.Serializable]
public class ControlFountainReadyEvent:IGameEvent//开始前的倒计时事件
{

}
[System.Serializable]
public class ControlFountainStartEvent:IGameEvent
{

}
[System.Serializable]
public class ControlFountainEndEvent:IGameEvent
{

}
[System.Serializable]
public class ControlFountainHit:IGameEvent//行人受伤事件
{

}
// ==================== 怪物追逐事件 ====================
[System.Serializable]
public class MonsterCatchEvent : IGameEvent { }

[System.Serializable]
public class ReviveEvent : IGameEvent { }

// ==================== 任务系统事件 ====================
[System.Serializable]
public class TaskStartEvent : IGameEvent
{
    public string TaskId;
    public string TaskName;
    public string TaskNumber;
    public int TargetCount;
    public string Description;
}

[System.Serializable]
public class TaskProgressEvent : IGameEvent
{
    public string TaskId;
    public int Amount;
}

[System.Serializable]
public class TaskCompleteEvent : IGameEvent
{
    public string TaskId;
}

public class FadeEvent:IGameEvent
{
    public float fadeInTime;
    public float fadeOutTime;
    public float duration;
    public Image fadeImage;
}

//public class FadeInEvent:IGameEvent
//{
//    public float fadeInTime;
//    public Image fadeImage;
//}

public class ValveFixedEvent:IGameEvent
{

}

// ==================== 音频系统事件 ====================
public enum AudioTrack { BGM, PlayerFootstep, Fountain1, Other, MonsterFootstep, Fountain2 }

[System.Serializable]
public class PlaySoundEvent : IGameEvent
{
    public AudioClip Clip;
    public AudioTrack Track;
    public float Volume = 1f;
}

//public class  NoteFinishReadEvent:IGameEvent
//{
//
//}

[System.Serializable]
public class PlayDoorSoundEvent : IGameEvent
{
    
}
