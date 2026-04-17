using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class MuteSoundPerform : DialoguePerform
    {
        private MuteData data;
        public override void Perform()
        {
            GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
            { Track = this.data.track });
            Debug.LogWarning("静音!!"); 
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as MuteData;
        }
    }
}
