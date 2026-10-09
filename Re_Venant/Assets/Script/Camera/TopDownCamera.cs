using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Height")]
    [SerializeField] private float cameraHeight = 12f;

    [Header("Follow")]
    [SerializeField] private float followSmoothTime = 0.12f;

    [Header("Mouse Look Ahead")]
    [SerializeField] private float maxLookAheadDistance = 4f;

    [SerializeField]
    private float mouseLookAheadStrength = 1f;

    private Vector3 followVelocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        // 화면 중앙
        Vector2 screenCenter = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        // 중앙에서 마우스까지의 거리
        Vector2 mouseOffset =
            mousePosition - screenCenter;

        // -1 ~ 1 범위로 변환
        Vector2 normalizedOffset = new Vector2(
            mouseOffset.x / (Screen.width * 0.5f),
            mouseOffset.y / (Screen.height * 0.5f)
        );

        normalizedOffset =
            Vector2.ClampMagnitude(
                normalizedOffset,
                1f
            );

        // 카메라의 화면 방향을 월드 방향으로 변환
        Vector3 cameraRight =
            Vector3.ProjectOnPlane(
                transform.right,
                Vector3.up
            ).normalized;

        Vector3 cameraUp =
            Vector3.ProjectOnPlane(
                transform.up,
                Vector3.up
            ).normalized;

        Vector3 lookAheadDirection =
            cameraRight * normalizedOffset.x +
            cameraUp * normalizedOffset.y;

        Vector3 lookAheadOffset =
            lookAheadDirection *
            maxLookAheadDistance *
            mouseLookAheadStrength;

        Vector3 targetPosition =
            target.position +
            Vector3.up * cameraHeight +
            lookAheadOffset;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref followVelocity,
                followSmoothTime
            );
    }
}