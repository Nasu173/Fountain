using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 对话移动演出,比如npc移动到指定位置
    /// </summary>
    public class WalkPerform : DialoguePerform
    {
        private WalkData data;
        public override void Perform()
        {
            data.npcMove.SetDuration(data.duration);
            data.npcMove.MoveToward(data.targetPosition);            
            data.anim.SetBool(data.walkAnimName,true);
            data.npcMove.Arrived += () => 
            { data.anim.SetBool(data.walkAnimName, false); };
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as WalkData;
        }
    }
}
