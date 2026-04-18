using Fountain.Dialogue;
using Fountain.InputManagement;
using Fountain.Player;
using System.Collections;
using System.Linq;
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
    public Vector3 monsterPos;
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

    public AudioTrack stopTrack;
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
        GameEventBus.Publish<TaskProgressEvent>(new TaskProgressEvent() 
        { Amount = 1, TaskId = 17.ToString() });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = this.stopTrack });//停下警报
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.BGM });

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
        player.LookAt(lookTarget.position, lookDuration);
        yield return new WaitForSeconds(lookDuration+0.1f);
        //黑屏一下

        GameEventBus.Publish<FadeEvent>(new FadeEvent()
        {
            fadeInTime = 0,
            duration = 0.5f,//硬编码
            fadeOutTime =0
        });
        ShowMonster();

        yield return new WaitForSeconds(delayBeforeBlack); 

        GameEventBus.Publish<FadeEvent>(new FadeEvent()
        {
            fadeInTime = this.fadeInTime,
            fadeImage = this.fadeImage,
            duration = this.duration,
            fadeOutTime = this.fadeOutTime
        });

        yield return new WaitForSeconds(delayAfterBlack);
        IPerformDataProvider[] datas = this.GetComponents<IPerformDataProvider>();
        DialogueManager.Instance.StartDialogue(dialogue,datas.ToList()); 
    }
    private void ShowMonster()
    {
        monster.gameObject.SetActive(true);
        Transform player = PlayerInstance.Instance.transform;
        monster.forward = -player.forward;
        //monster.position = player.position + (-1 * player.forward * monsterDistance);
        monster.transform.position = monsterPos;
    }
}
