using Fountain.InputManagement;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerChaseAfterFix : MonoBehaviour
{
    //修好水阀后完成相关演出效果
    public string taskCompleteId;
    public int targetAmount;
    private int currentAmount = 0;

    //场景物体
    public GameObject wall;
    public GameObject wallLight;

    [SerializeField] private AudioClip audioClip;
    


    //GameEventBus.Publish(new TaskCompleteEvent { TaskId = TaskId
    private void OnEnable()
    {
        GameEventBus.Subscribe<TaskProgressEvent>(Count); 
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<TaskProgressEvent>(Count); 
    }

    /*
     修好水阀,
怪物走出来，玩家看怪物,
怪物转向玩家,怪物播吼叫的动画，玩家启用移动
     */
    private void Count(TaskProgressEvent e)
    {
        if (e.TaskId==taskCompleteId)
        {
            currentAmount += e.Amount;
            if (currentAmount>=targetAmount)
            {
                StartCoroutine(Perform());
            }
        }        
    }
    private IEnumerator Perform()
    {
        //玩家禁用输入
        CharacterInputProvider moveInput = GameInputManager.Instance.
            GetProvider<CharacterInputProvider>();
        moveInput.enabled = false;
        PlayerSightInputProvider sightInput = GameInputManager.Instance.GetProvider
            <PlayerSightInputProvider>();
        PlayerMove player = PlayerInstance.Instance.GetComponent<PlayerMove>();
        sightInput.enabled = false;
        //先把墙消失,灯打开,视角看向那个洞,
        wallLight.SetActive(true);
        wall.SetActive(false);

        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        player.LookAt(wall.transform.position, 1f);

        yield return new WaitForSeconds(1f);

        GameEventBus.Publish<ScriptTriggerEvent>
            (new ScriptTriggerEvent() { TriggerId = "15" });


        yield return new WaitForSeconds(1f);
        //玩家启用输入
        moveInput.enabled = true;
        sightInput.enabled = true;
    }
}
