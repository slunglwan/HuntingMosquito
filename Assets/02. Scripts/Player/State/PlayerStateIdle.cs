using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateIdle : PlayerState, ICharacterState
{
    public PlayerStateIdle(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        animator.SetBool(PlayerAniParamIdle, true);

        // Player Input에 대한 액션 할당
        playerInput.actions["Attack"].performed += Attack;
    }

    public void Exit()
    {
        // Idle 애니메이션 종료
        animator.SetBool(PlayerAniParamIdle, false);

        // Player Input에 대한 액션 해제
        playerInput.actions["Attack"].performed -= Attack;
    }

    public void Update()
    {
        if (playerInput.actions["Move"].IsPressed())
        {
            playerController.SetState(EPlayerState.Move);
        }
    }
}
