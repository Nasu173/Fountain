using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 播放声音的演出效果(比如打电话的音效)
    /// </summary>
    public class PlaySoundPerform : DialoguePerform
    {
        private SoundData data;
        public override void Perform()
        {
            //var clip = Resources.Load<AudioClip>(data.soundStr);
            //if (clip == null) { Debug.LogWarning($"[PlaySoundPerform] 找不到音频: {data.soundStr}"); return; }
                
            GameEventBus.Publish(new PlaySoundEvent()
            {
                Clip = data.clip, 
                Track = data.track,
                Volume=data.volume,
            });
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as SoundData;
        }
    }
}
