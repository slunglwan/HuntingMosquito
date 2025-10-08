using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, ISkillObserver<GameObject>
{
    [SerializeField] Transform headTransform;
    private float velocityY;
    [Header("이동")]
    [SerializeField]
    [Range(1, 5)] private float breakForce = 1f;
    [SerializeField] private Skill_Punch skill_Punch;

    // 컴포넌트 캐싱
    private Animator animator;
    private PlayerInput playerInput;
    private CharacterController cc;


    // 상태 정보
    private Dictionary<EPlayerState, ICharacterState> states;
    private List<ISkillObserver<GameObject>> observers = new List<ISkillObserver<GameObject>>();
    public EPlayerState State { get; private set; }
    public float BreakForce => breakForce;

    private void Awake()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cc = GetComponent<CharacterController>();

        // 상태 객체 초기화
        var playerStateIdle = new PlayerStateIdle(this, animator, playerInput);
        var playerStateMove = new PlayerStateMove(this, animator, playerInput, cc);
        var playerStateAttack = new PlayerStateAttack(this, animator, playerInput);
        var playerStateHit = new PlayerStateHit(this, animator, playerInput);
        var playerStateDead = new PlayerStateDead(this, animator, playerInput);

        states = new Dictionary<EPlayerState, ICharacterState>
        {
            { EPlayerState.Idle, playerStateIdle },
            { EPlayerState.Move, playerStateMove },
            { EPlayerState.Attack, playerStateAttack },
            { EPlayerState.Hit, playerStateHit },
            { EPlayerState.Dead, playerStateDead },
        };

        // 상태 초기화
        SetState(EPlayerState.Idle);

        //Cursor 숨기기
        playerInput.actions["Cursor"].performed += (value) => GameManager.Instance.SetCursorLock();
    }

    private void Start()
    {
        skill_Punch.Subscribe(this);
    }

    private void OnEnable()
    {
        // 카메라 초기화
        playerInput.camera = Camera.main;
        if (playerInput.camera != null)
        {
            playerInput.camera.GetComponent<CameraController>().SetTarget(headTransform, playerInput);
        }
    }

    public void SetState(EPlayerState state)
    {
        if (State == state) return;
        if (State != EPlayerState.None) states[State].Exit();
        State = state;
        if (State != EPlayerState.None) states[State].Enter();

    }

    private void Update()
    {
        if (State != EPlayerState.None)
        {
            states[State].Update();
        }
    }

    public void OnNext(GameObject value)
    {
        var enemyController = value.GetComponent<EnemyController>();
        if (enemyController)
        {
            enemyController.SetHit(30);
        }
    }

    public void Punch()
    {
        skill_Punch.PerformAttack();
    }

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {

    }
}
