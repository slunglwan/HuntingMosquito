using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateAttack : PlayerState, ICharacterState
{
    public PlayerStateAttack(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        if(!_playerController.IsWeaponEquip) _animator.SetTrigger(PlayerAniParamPunch);
        else _animator.SetTrigger(PlayerAniParamSwing);
    }

    public void Exit()
    {
        // Player Input에 대한 액션 해제
        _playerInput.actions["Attack"].performed -= Attack;
    }

    public void Update()
    {

    }
}
