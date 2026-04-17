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
    public float lookDuration;//玩家转视角的持续时间
    [SerializeField] private AudioClip brickClip;
    //怪物演出
    public GameObject monster;
    public Animator animMonster;
    public string runAnimName;
    public string yeildAnimName;
    public Vector3 spawnPosition;//怪物从哪里走出来
    public Vector3 endPosition;

    public float moveDuration=1f;
    public float rotationDuration = 1f;

    //怪物追逐
    [SerializeField] private MonsterChase monsterChase;
    


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
修好水阀,玩家禁用输入
先把墙消失,灯打开,视角看向那个洞,
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
            Clip = brickClip,
            Track = AudioTrack.Other
        });

        player.LookAt(wall.transform.position, lookDuration);

        yield return new WaitForSeconds(lookDuration);

        GameEventBus.Publish<ScriptTriggerEvent>
            (new ScriptTriggerEvent() { TriggerId = "15" });

        //怪物走出来
        monster.SetActive(true);
        yield return StartCoroutine(MoveMonster());
        //玩家看怪物,
        player.LookAt(monster.transform.position, lookDuration);
        yield return new WaitForSeconds(lookDuration);
        //怪物转向玩家,怪物播吼叫的动画，玩家启用移动
        yield return StartCoroutine(RotateMonster());
        animMonster.SetTrigger(yeildAnimName);

        yield return new WaitForSeconds(1f);//随便给的1秒
        //玩家启用输入
        moveInput.enabled = true;
        sightInput.enabled = true;

        //怪物追逐
        monsterChase.StartChase();

    }
        private IEnumerator MoveMonster()
        {
            float elapsed = 0;
            this.animMonster.SetBool(runAnimName, true);
            while (elapsed<moveDuration)
            {
                float t = elapsed /moveDuration;
                this.transform.position = Vector3.Lerp(spawnPosition, endPosition, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            this.animMonster.SetBool(runAnimName, false);
        }
    private IEnumerator RotateMonster()
    {
        Transform player = PlayerInstance.Instance.transform;
        Quaternion targetRotation = Quaternion.LookRotation
            (player.position - monster.transform.position);
        Quaternion startRotation = monster.transform.rotation;
        float elapsed = 0;
        while (elapsed<rotationDuration)
        {
            monster.transform.rotation= Quaternion.Lerp
                (startRotation,targetRotation , elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        monster.transform.rotation = targetRotation;

    }
}
