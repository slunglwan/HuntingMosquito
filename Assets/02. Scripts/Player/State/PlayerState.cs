using static Constants;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerController _playerController;
    protected Animator _animator;
    protected PlayerInput _playerInput;

    public PlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
    {
        _playerController = playerController;
        _animator = animator;
        _playerInput = playerInput;
    }

    protected void Attack(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != EGameState.Play) return;
        _playerController.SetState(EPlayerState.Attack);
    }

    protected void Rotate(float x, float z)
    {
        if (_playerInput.camera != null)
        {
            var cameraTransform = _playerInput.camera.transform;
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraForward.y = 0f;

            var moveDirection = cameraForward * z + cameraRight * x;

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                _playerController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
}
