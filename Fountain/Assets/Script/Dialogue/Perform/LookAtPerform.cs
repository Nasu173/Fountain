using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 实现看向物体的演出,比如看向NPC
    /// </summary>
    public class LookAtPerform : DialoguePerform
    {
        private LookData data = null;
        public override void Perform()
        {
            if (data==null)
            {
                return;
            }
            data.playerMove.LookAt(data.target.position, data.transitionSpeed);
            data.cam.m_Lens.FieldOfView = data.targetFOV;
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as LookData;
        }
    }
}
