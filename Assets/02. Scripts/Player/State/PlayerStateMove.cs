using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateMove : PlayerState, ICharacterState
{
    private float moveSpeed;
    private float minSpeed = 3f;
    private float maxSpeed = 9f;
    private float acceleration = 0.1f;
    private float velocityY;
    private CharacterController cc;

    public PlayerStateMove(PlayerController playerController, Animator animator, PlayerInput playerInput, CharacterController cc)
        : base(playerController, animator, playerInput)
    {
        this.cc = cc;
    }

    public void Enter()
    {
        animator.SetBool(PlayerAniParamMove, true);

        // Player Input에 대한 액션 할당
        playerInput.actions["Attack"].performed += Attack;

        // moveSpeed 초기화
        moveSpeed = minSpeed;
    }

    public void Exit()
    {
        // Move 애니메이션 종료
        animator.SetBool(PlayerAniParamMove, false);

        // Player Input에 대한 액션 해제
        playerInput.actions["Attack"].performed -= Attack;
    }

    public void Update()
    {
        // 캐릭터 방향 설정
        var moveVector = playerInput.actions["Move"].ReadValue<Vector2>();

        if (moveVector != Vector2.zero)
        {
            Rotate(moveVector.x, moveVector.y);
            var movePosition = MoveDirection(moveVector.x, moveVector.y) * Time.deltaTime * moveSpeed;
            velocityY += Gravity * Time.deltaTime;
            movePosition.y = velocityY * Time.deltaTime;
            cc.Move(movePosition);
        }
        else
        {
            playerController.SetState(EPlayerState.Idle);
        }

        // 이동 스피드 설정
        var isRun = playerInput.actions["Run"].IsPressed();
        if (isRun && moveSpeed < maxSpeed)
        {
            moveSpeed += acceleration;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }
        else if (!isRun && moveSpeed > minSpeed)
        {
            moveSpeed -= acceleration * playerController.BreakForce;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }
        animator.SetFloat(PlayerAniParamMoveSpeed, moveSpeed);
    }

    private Vector3 MoveDirection(float x, float z)
    {
        if (playerInput.camera != null)
        {
            var cameraTransform = playerInput.camera.transform;
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0f;

            var moveDirection = cameraForward * z + cameraRight * x;
            moveDirection.Normalize();
            return moveDirection;
        }
        else return Vector3.zero;
    }
}
