using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 仅渐变视野大小的演出效果
    /// </summary>
    public class SetFOVPerform : DialoguePerform
    {
        private SetFOVData data;
        public override void Perform()
        {
            data.StartCoroutine(Restore(data.cam.m_Lens.FieldOfView, data.targetFOV));    
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as SetFOVData;
        }
        private IEnumerator Restore(float start, float end)
        {
            float elapsed = 0;
            while (elapsed < data.duration)
            {
                elapsed += Time.deltaTime;
                data.cam.m_Lens.FieldOfView = Mathf.Lerp(start, end, elapsed / data.duration);
                yield return null;
            }
            data.cam.m_Lens.FieldOfView = data.targetFOV;
        }

    }
}
