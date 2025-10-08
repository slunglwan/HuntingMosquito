using static Constants;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerController playerController;
    protected Animator animator;
    protected PlayerInput playerInput;

    public PlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
    {
        this.playerController = playerController;
        this.animator = animator;
        this.playerInput = playerInput;
    }

    protected void Attack(InputAction.CallbackContext context)
    {
        playerController.SetState(EPlayerState.Attack);
    }

    protected void Rotate(float x, float z)
    {
        if (playerInput.camera != null)
        {
            var cameraTransform = playerInput.camera.transform;
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraForward.y = 0f;

            var moveDirection = cameraForward * z + cameraRight * x;

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                playerController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
}
