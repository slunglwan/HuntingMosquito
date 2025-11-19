using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateVictory : PlayerState, ICharacterState
{
    public PlayerStateVictory(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerAniParamVictory);
    }

    public void Exit() { }

    public void Update() { }
}
