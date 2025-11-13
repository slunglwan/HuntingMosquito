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

    }

    public void Update()
    {

    }
}
