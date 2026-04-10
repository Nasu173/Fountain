using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class ApprenticeLeavePerform : DialoguePerform
    {
        private ApprenticeLeaveData data;
        public override void Perform()
        {
            GameEventBus.Publish<FadeEvent>(new FadeEvent()
            {
                fadeImage = data.fadeImage,
                fadeInTime = data.fadeInTime,
                fadeOutTime = data.fadeOutTime,
                duration = data.duration
            });
            data.StartCoroutine(ApprenticeLeave());
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as ApprenticeLeaveData;
        }
        private IEnumerator ApprenticeLeave()
        {
            yield return new WaitForSeconds(data.fadeInTime);
            data.apprentice.gameObject.SetActive(false);
            GameEventBus.Publish<PlayDoorSoundEvent>(null);
        }
    }
}
