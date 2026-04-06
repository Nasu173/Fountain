using System.Collections;
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
   // [SerializeField] private string animExecuteTrigger = "Execute";
    [SerializeField] private float executionAnimDuration = 3f;

    private enum State { Idle, Chasing, Executing, Done }
    private State _state = State.Idle;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;

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

        //_animator.SetTrigger(animExecuteTrigger);
        yield return new WaitForSeconds(executionAnimDuration);

        GameEventBus.Publish(new MonsterCatchEvent());
        _state = State.Done;
    }
}
