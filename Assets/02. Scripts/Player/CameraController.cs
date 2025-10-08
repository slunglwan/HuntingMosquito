using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask obstacleLayerMask;

    private Transform target;
    private Vector2 lookVector;
    private float azimuthAngle;
    private float polarAngle;

    private void LateUpdate()
    {
        if (target != null)
        {
            // 마우스 x, y 값을 이용하여 카메라 이동
            azimuthAngle += lookVector.x * rotationSpeed * Time.deltaTime;
            polarAngle -= lookVector.y * rotationSpeed * Time.deltaTime;
            polarAngle = Mathf.Clamp(polarAngle, -20f, 60f);

            // 벽 감지
            var currentDistance = AdjustCameraDistance();

            // 카메라 위치 설정
            var cartesianPosition = GetCameraPosition(currentDistance, polarAngle, azimuthAngle);
            transform.position = target.position - cartesianPosition;
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform target, PlayerInput playerInput)
    {
        this.target = target;

        // 카메라 초기 위치 설정
        var cartesianPosition = GetCameraPosition(distance, polarAngle, azimuthAngle);
        transform.position = target.position - cartesianPosition;
        transform.LookAt(target);

        // 마우스 이동 
        playerInput.actions["Look"].performed += OnActionLook;
        playerInput.actions["Look"].canceled += OnActionLook;
    }

    private void OnActionLook(InputAction.CallbackContext context)
    {
        lookVector = context.ReadValue<Vector2>();
    }

    private Vector3 GetCameraPosition(float r, float polarAngle, float azizmuthAngle)
    {
        float b = r * Mathf.Cos(polarAngle *  Mathf.Deg2Rad);

        float x = b * Mathf.Cos(azizmuthAngle * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(polarAngle * Mathf.Deg2Rad) * -1;
        float z = b * Mathf.Sin(azizmuthAngle *Mathf.Deg2Rad);

        return new Vector3(x, y, z);
    }

    private float AdjustCameraDistance()
    {
        var currentDistance = distance;

        Vector3 direction = GetCameraPosition(1, polarAngle, azimuthAngle).normalized;
        RaycastHit hit;

        if (Physics.Raycast(target.position, -direction, out hit, distance, obstacleLayerMask))
        {
            float offset = 0.3f;
            currentDistance = hit.distance - offset;
            currentDistance = Mathf.Max(currentDistance, 0.5f);
        }

        return currentDistance;
    }
}
