using Fountain.Dialogue;
using Fountain.Fountain;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeath : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip fountainSound; 

    public FountainController fountain;
    public float animDelay;//播放动画的延迟
    public float turnOffDelay;
    public Animator anim;
    public DialogueSequence dialogue;
    public NPCMove npc;

    private void Start()
    {
        npc.Arrived += () => { StartCoroutine(Trigger()); };
    }
    private IEnumerator Trigger()
    {
        fountain.TurnOn();
        yield return new WaitForSeconds(animDelay);
        anim.SetTrigger("Die");
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = deathSound,
            Track = AudioTrack.Other
        });
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = fountainSound,
            Track = AudioTrack.Fountain1
        });
        DialogueManager.Instance.StartDialogue(dialogue,null);
        yield return new WaitForSeconds(turnOffDelay);
        fountain.TurnOff();
    }

}
