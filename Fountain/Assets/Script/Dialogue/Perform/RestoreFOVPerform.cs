using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class RestoreFOVPerform : DialoguePerform
    {
        private RestoreFOVProvider data;
        public override void Perform()
        {
            data.StartCoroutine(Restore(data.cam.m_Lens.FieldOfView, data.targetFOV));    
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as RestoreFOVProvider;
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

        }

    }
}
