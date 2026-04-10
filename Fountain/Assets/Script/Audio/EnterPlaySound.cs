using UnityEngine;

public class EnterPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
    }
}
