using Fountain.Dialogue;
using Fountain.InputManagement;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ScareTrigger : MonoBehaviour
{
    //玩家触发Trigger时突脸
    [Header("强制转向所需数据")]
    [SerializeField]
    private Transform lookTarget;
    [SerializeField]
    private float lookDuration;

    public Transform monster;
    [SerializeField]
    private float monsterDistance;
    [SerializeField]
    private float delayBeforeBlack;

    [Header("黑屏效果设置")]
    public float fadeInTime;
    public float fadeOutTime;
    public float duration=float.MaxValue;//简单一点,只有淡入
    public Image fadeImage;
    
    public float delayAfterBlack;
    public DialogueSequence dialogue;
    private bool triggered=false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!triggered)
        {
            StartCoroutine(Scare());
            triggered = true;
        }
    }
    private IEnumerator Scare()//突脸
    {
        GameInputManager.Instance.GetProvider<CharacterInputProvider>()
            .enabled = false;
        GameInputManager.Instance.GetProvider<PlayerSightInputProvider>()
            .enabled = false;
            PlayerMove player = PlayerInstance.Instance.GetComponent<PlayerMove>();

            //站起来
            if (player.IsCrouched())
            {
                 player.SwitchCrouch();
                 while (player.IsCrouching())
                 {
                     yield return null; 
                 }
            }
        ShowMonster();
        player.LookAt(lookTarget.position, lookDuration);
        yield return new WaitForSeconds(lookDuration+delayBeforeBlack);

        GameEventBus.Publish<FadeEvent>(new FadeEvent()
        {
            fadeInTime = this.fadeInTime,
            fadeImage = this.fadeImage,
            duration = this.duration,
            fadeOutTime = this.fadeOutTime
        });

        yield return new WaitForSeconds(delayAfterBlack);
        DialogueManager.Instance.StartDialogue(dialogue,null); 
    }
    private void ShowMonster()
    {
        monster.gameObject.SetActive(true);
        Transform player = PlayerInstance.Instance.transform;
        monster.forward = player.forward;
        monster.position = player.position + (-1 * player.forward * monsterDistance);
    }
}
