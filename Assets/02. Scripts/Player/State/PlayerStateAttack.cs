using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateAttack : PlayerState, ICharacterState
{
    public PlayerStateAttack(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        animator.SetTrigger(PlayerAniParamAttack);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}
