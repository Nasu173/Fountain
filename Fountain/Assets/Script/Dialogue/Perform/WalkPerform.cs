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
            data.npcMove.SetSpeed(data.speed);
            data.npcMove.MoveToward(data.targetPosition);            
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as WalkData;
        }
    }
}
