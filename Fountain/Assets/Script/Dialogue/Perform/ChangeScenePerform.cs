using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class ChangeScenePerform : DialoguePerform
    {
        private ChangeSceneData data;
        public override void Perform()
        {
            data.StartCoroutine(DelayChangeScene());
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as ChangeSceneData;
        }
        private IEnumerator DelayChangeScene()
        {
            yield return new WaitForSeconds(data.delay);
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = data.loadSceneAddress,
                Additive = true,
                SceneToUnload = data.unloadSceneAddress
            });
            GameEventBus.Publish<FadeEvent>(new FadeEvent()
            {
                fadeInTime = 0,
                duration = 1,
                fadeOutTime = 1
            });
        }
    }
}
