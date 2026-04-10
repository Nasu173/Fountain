using UnityEngine;

public class EnterPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool triggerOnce;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });

        // 如果设置为只触发一次，禁用此触发器
        if (triggerOnce)
        {
            enabled = false; // 禁用此组件，不再检测
        }
    }
}
