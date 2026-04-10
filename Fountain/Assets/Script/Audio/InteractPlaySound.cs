using UnityEngine;

public class InteractPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private KeyCode key = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(key))
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
    }
}
