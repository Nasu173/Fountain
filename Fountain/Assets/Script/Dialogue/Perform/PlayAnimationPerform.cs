using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 对话时播放动画的演出效果,由于现在没用动画资源,不太好测试
    /// </summary>
    public class PlayAnimationPerform : DialoguePerform
    {
        public AnimationPlayData data;
        public override void Perform()
        {
            Debug.LogFormat("播放动画{0}", data.animName);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as AnimationPlayData;
        }
    }
}
