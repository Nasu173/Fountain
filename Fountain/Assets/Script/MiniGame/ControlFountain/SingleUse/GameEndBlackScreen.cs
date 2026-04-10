using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndBlackScreen : MonoBehaviour
{
    public float duration = 0.2f;
    private void OnEnable()
    {
        GameEventBus.Subscribe<ControlFountainEndEvent>(Black);
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<ControlFountainEndEvent>(Black);
    }
    private void Black(ControlFountainEndEvent e)
    {
        GameEventBus.Publish<FadeEvent>(new FadeEvent()
        {
            fadeInTime = 0,
            fadeOutTime = 0,
            duration = duration
        });
    }
}
