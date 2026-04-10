using Fountain.Dialogue;
using Fountain.Fountain;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeath : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip; 

    public FountainController fountain;
    public float animDelay;//播放动画的延迟
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
            Clip = audioClip,
            Track = AudioTrack.Other
        });
        DialogueManager.Instance.StartDialogue(dialogue,null);
    }

}
