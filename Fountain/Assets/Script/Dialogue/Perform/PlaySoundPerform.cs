using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 播放声音的演出效果(比如打电话的音效),由于没有播放音乐音效的接口,不太好测试
    /// </summary>
    public class PlaySoundPerform : DialoguePerform
    {
        private SoundData data;
        public override void Perform()
        {
            Debug.LogFormat("播放声音:{0}", data.soundStr);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as SoundData;
        }
    }
}
