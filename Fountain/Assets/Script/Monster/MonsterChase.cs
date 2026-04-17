using System.Collections;
using Fountain.InputManagement;
using Fountain.Player;
using UnityEngine;
using UnityEngine.AI;

public class MonsterChase : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float executionDistance = 1.2f;
    //[SerializeField] private string animSpeedParam = "Speed";
    [SerializeField] private string animRunParam = "Run";
    [SerializeField] private string animExecuteTrigger = "Execute";
    [SerializeField] private float executionAnimDuration = 3f;
    [SerializeField] private string sceneAddress = "Underground";

    private enum State { Idle, Chasing, Executing, Done }
    private State _state = State.Idle;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;
    public Transform headPos;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.speed = chaseSpeed;
        _agent.isStopped = true;
    }

    public void StartChase()
    {
        if (_player == null)
            _player = PlayerInstance.Instance.transform;
        _state = State.Chasing;
        _agent.isStopped = false;
        _animator.SetBool(animRunParam, true);
    }

    public void StopChase()
    {
        if (_state == State.Executing || _state == State.Done) return;
        _state = State.Idle;
        _agent.isStopped = true;
        _animator.SetBool(animRunParam, false);
        //_animator.SetFloat(animSpeedParam, 0f);
    }

    private void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.K)) StartChase();
            if (Input.GetKeyDown(KeyCode.L)) StopChase();
        }

        if (_state != State.Chasing) return;

        _agent.SetDestination(_player.position);
        //_animator.SetFloat(animSpeedParam, _agent.velocity.magnitude);

        if (Vector3.Distance(transform.position, _player.position) <= executionDistance)
            StartCoroutine(ExecutePlayer());
    }

    private IEnumerator ExecutePlayer()
    {
        _state = State.Executing;
        _agent.isStopped = true;
        //_animator.SetFloat(animSpeedParam, 0f);

        // 朝向玩家（仅 Y 轴）
        Vector3 dir = (_player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);


        // 禁用玩家控制
        var playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
        var playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>();
        if (playerMove != null) playerMove.enabled = false;
        if (playerSight != null) playerSight.enabled = false;

        //玩家看向怪物
        playerMove.LookAt(headPos.position, 1); //随手写的1秒
        yield return new WaitForSeconds(1);//随手写的1秒

        _animator.SetTrigger(animExecuteTrigger);
        yield return new WaitForSeconds(executionAnimDuration);

        if (playerMove != null) playerMove.enabled = false;//不知为何,上面的禁用不生效到这里
        if (playerSight != null) playerSight.enabled = false;
        GameInputManager.Instance.GetProvider<PauseInputProvider>().enabled = false;

        GameEventBus.Publish<RespawnEvent>(new RespawnEvent()
        {
            backgroundColor = Color.black,
            textColor = Color.red,
            fadeInTime = 1f,
            sceneToLoad = this.sceneAddress
        });
        GameEventBus.Publish(new MonsterCatchEvent());
        _state = State.Done;
    }
}
