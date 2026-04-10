using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class BlackScreenPerform : DialoguePerform
    {
        private BlackScreenDataProvider data;
        public override void Perform()
        {
            GameEventBus.Publish<FadeEvent>(new FadeEvent()
            {
                fadeImage = data.fadeImage,
                fadeInTime = data.fadeInTime,
                fadeOutTime = data.fadeOutTime,
                duration = data.duration
            });
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as BlackScreenDataProvider;
        }
    }
}
