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
    private SkillPunch _skillPunch;
    [SerializeField] private Transform weaponPos;
    private SkillSweater _skillSweater;

    // 컴포넌트 캐싱
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController cc;


    // 상태 정보
    private Dictionary<EPlayerState, ICharacterState> states;
    private List<ISkillObserver<GameObject>> observers = new List<ISkillObserver<GameObject>>();
    public EPlayerState State { get; private set; }
    public float BreakForce => breakForce;
    public bool IsWeaponEquip { get; private set; }

    private void Awake()
    {
        // 컴포넌트 초기화
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        cc = GetComponent<CharacterController>();

        // 상태 객체 초기화
        var playerStateIdle = new PlayerStateIdle(this, _animator, _playerInput);
        var playerStateMove = new PlayerStateMove(this, _animator, _playerInput, cc);
        var playerStateAttack = new PlayerStateAttack(this, _animator, _playerInput);
        var playerStateHit = new PlayerStateHit(this, _animator, _playerInput);
        var playerStateDead = new PlayerStateDead(this, _animator, _playerInput);
        var playerStateVictory = new PlayerStateVictory(this, _animator, _playerInput);
        SetWeaponEquip(false);

        states = new Dictionary<EPlayerState, ICharacterState>
        {
            { EPlayerState.Idle, playerStateIdle },
            { EPlayerState.Move, playerStateMove },
            { EPlayerState.Attack, playerStateAttack },
            { EPlayerState.Hit, playerStateHit },
            { EPlayerState.Dead, playerStateDead },
            { EPlayerState.Victory, playerStateVictory },
        };

        StageManager.OnStageClear += () => SetState(EPlayerState.Victory);

        // 상태 초기화
        SetState(EPlayerState.Idle);

        //Cursor 숨기기
        _playerInput.actions["Cursor"].performed += (value) => GameManager.Instance.SetCursorLock();
    }

    private void Start()
    {
        _skillPunch = GetComponentInChildren<SkillPunch>();
        _skillPunch.Subscribe(this);
    }

    public void SetCamera()
    {
        // 카메라 초기화
        _playerInput.camera = Camera.main;
        if (_playerInput.camera != null)
        {
            _playerInput.camera.GetComponent<CameraController>().SetTarget(headTransform, _playerInput);
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
        if (GameManager.Instance.GameState != EGameState.Play) return;

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
            int dmg = IsWeaponEquip ? 60 : 30;
            enemyController.SetHit(dmg);
        }
    }

    public void SetWeaponEquip(bool isEquip)
    {
        IsWeaponEquip = isEquip;
    }

    public void Punch()
    {
        StartCoroutine(_skillPunch.PerformAttack());
    }

    public void Swing()
    {
        StartCoroutine(_skillSweater.PerformAttack());
    }

    public void EquipWeapon(GameObject weaponObj)
    {
        weaponObj.transform.SetParent(weaponPos);
        SetWeaponEquip(true);
        _skillSweater = weaponObj.GetComponent<SkillSweater>();
        _skillSweater.Subscribe(this);        
    }

    public void UnEquipWeapon()
    {
        if(_skillSweater != null)
        {
            _skillSweater.UnSubscribe(this);
            Destroy(_skillSweater.gameObject);
            _skillSweater = null;
            SetWeaponEquip(false);
        }
    }

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {

    }
}
