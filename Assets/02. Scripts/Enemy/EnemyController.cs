using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IPoolable
{
    [Header("AI")]
    [SerializeField] private float actionWaitTime = 1f;
    [SerializeField] private float moveChance = 60f;
    [SerializeField] private float idleChance = 30f;
    [SerializeField] private float restChance = 30f;
    [SerializeField] private float safeDistance = 30f;
    [SerializeField] private float playerDetectionDistance = 20f;
    [SerializeField] private float runDistance = 10f;
    [SerializeField] private float patrolDistance = 10f;
    [SerializeField] private LayerMask detectionTargetLayerMask;

    // 컴포넌트 캐싱
    private Animator animator;
    private NavMeshAgent agent;

    // 상태 정보
    public EEnemyState State {  get; private set; }
    private float hp = 50;
    private float curHp;
    private Dictionary<EEnemyState, ICharacterState> states;
    private readonly Collider[] detectionResults = new Collider[1];
    public static event Action<GameObject> OnMosquitoKilled;

    // AI 관련
    public float ActionWaitTime => actionWaitTime;
    public float MoveChance => moveChance;
    public float IdleChance => idleChance;
    public float RestChance => restChance;
    public float SafeDistance => safeDistance;
    public float PlayerDetectionDistance => playerDetectionDistance;
    public float RunDistance => runDistance;
    public float PatrolDistance => patrolDistance;
    public bool IsSafe { get; set; } = true;

    private void Awake()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent 설정
        agent.updatePosition = true;
        agent.updateRotation = true;

        // 플레이어 할당
        var target = GameObject.FindGameObjectWithTag("Player");

        // 상태 설정
        curHp = hp;
        var enemyStateIdle = new EnemyStateIdle(this, animator, agent, target.transform);
        var enemyStateMove = new EnemyStateMove(this, animator, agent, target.transform);
        var enemyStateRest = new EnemyStateRest(this, animator, agent, target.transform);
        var enemyStateHit = new EnemyStateHit(this, animator, agent, target.transform);
        var enemyStateDead = new EnemyStateDead(this, animator, agent, target.transform);

        states = new Dictionary<EEnemyState, ICharacterState>
        {
            { EEnemyState.Idle, enemyStateIdle },
            { EEnemyState.Move, enemyStateMove },
            { EEnemyState.Rest, enemyStateRest },
            { EEnemyState.Hit, enemyStateHit },
            { EEnemyState.Dead, enemyStateDead },
        };
    }

    public void StartSMB()
    {
        IsSafe = true;
        SetState(EEnemyState.Idle);
    }    

    public void RestoreHp(float heal)
    {
        curHp += heal;
        if(curHp > hp)
            curHp = hp;
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        if (State != EEnemyState.None) states[State].Exit();
        State = state;
        if (State != EEnemyState.None) states[State].Enter();
    }

    private void OnDrawGizmos()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionDistance);

        // 달리기 결정 감지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, runDistance);

        // Agent 목적지 표시
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(agent.destination, 0.5f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }

    public bool DetectionTargetInCircle(float distance)
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, distance, detectionResults, detectionTargetLayerMask);

        bool detected = count > 0;
        Array.Clear(detectionResults, 0, count); // 탐지 후 캐시 초기화
        return detected;
    }

    public void SetHit(int damage)
    {
        if (State == EEnemyState.Dead) return; // 이미 사망한 경우 무시

        curHp -= damage;
        if (curHp <= 0)
        {
            SetState(EEnemyState.Dead);
            OnMosquitoKilled?.Invoke(gameObject);
        }
        else
            SetState(EEnemyState.Hit);
    }

    public void OnSpawned(Vector3 spawnPos)
    {
        // NavMesh 안전 배치(아래 참고) 
        PlaceOnNavMesh(spawnPos);
        curHp = hp;
        agent.updatePosition = true;
        agent.updateRotation = true;
        GetComponent<Collider>().enabled = true;
        IsSafe = true;
        SetState(EEnemyState.Idle);
    }

    public void OnDespawned()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    /// <summary>
    /// 스폰 좌표가 NavMesh위에 있도록 지정하는 함수
    /// </summary>
    /// <param name="spawnPos"></param>
    private void PlaceOnNavMesh(Vector3 spawnPos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
        {
            // agent가 활성화된 상태라면 Warping 권장
            if (agent != null && agent.isOnNavMesh)
                agent.Warp(hit.position);
            else
                transform.position = hit.position;
        }
        else
        {
            // 샘플 실패 시 fallback
            transform.position = spawnPos;
        }
    }
}
