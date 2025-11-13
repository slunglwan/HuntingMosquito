using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateIdle : PlayerState, ICharacterState
{
    public PlayerStateIdle(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetBool(PlayerAniParamIdle, true);

        // Player Input에 대한 액션 할당
        _playerInput.actions["Attack"].performed += Attack;
    }

    public void Exit()
    {
        // Idle 애니메이션 종료
        _animator.SetBool(PlayerAniParamIdle, false);

        // Player Input에 대한 액션 해제
        _playerInput.actions["Attack"].performed -= Attack;
    }

    public void Update()
    {
        if (_playerInput.actions["Move"].IsPressed())
        {
            _playerController.SetState(EPlayerState.Move);
        }
    }
}
