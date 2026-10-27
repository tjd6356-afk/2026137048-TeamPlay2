using UnityEngine;

public class MonsterCameraBillboard : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Center Facing")]
    [Tooltip("화면 중앙에서 이 범위 안에 있으면 카메라를 완전히 정면으로 봅니다.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float centerDeadZone = 0.12f;

    [Header("Side Tilt")]
    [Tooltip("화면 좌우에 있을 때 최대 얼마나 비스듬하게 보일지")]
    [Range(0f, 60f)]
    [SerializeField] private float maxSideAngle = 30f;

    [Tooltip("좌우 기울어지는 방향이 반대라면 체크")]
    [SerializeField] private bool invertSideAngle = false;

    [Header("Smooth")]
    [SerializeField] private float rotationSpeed = 12f;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        UpdateFacing();
    }

    private void UpdateFacing()
    {
        // =========================================
        // 1. 몬스터가 화면의 어느 위치에 있는지 계산
        // =========================================

        Vector3 viewportPosition =
            targetCamera.WorldToViewportPoint(
                transform.position
            );

        // viewport:
        //
        // 왼쪽 = 0
        // 중앙 = 0.5
        // 오른쪽 = 1

        float horizontalPosition =
            (viewportPosition.x - 0.5f) * 2f;

        horizontalPosition =
            Mathf.Clamp(
                horizontalPosition,
                -1f,
                1f
            );

        // =========================================
        // 2. 중앙에서는 기울기 0
        // =========================================

        float absHorizontal =
            Mathf.Abs(horizontalPosition);

        float sideStrength = 0f;

        if (absHorizontal > centerDeadZone)
        {
            sideStrength =
                Mathf.InverseLerp(
                    centerDeadZone,
                    1f,
                    absHorizontal
                );
        }

        // =========================================
        // 3. 좌우 기울기 계산
        // =========================================

        float sideAngle =
            horizontalPosition *
            maxSideAngle *
            sideStrength;

        if (invertSideAngle)
        {
            sideAngle *= -1f;
        }

        // =========================================
        // 4. 먼저 Canvas가 카메라를 완전히 바라보게 함
        // =========================================

        Quaternion cameraFacingRotation =
            Quaternion.LookRotation(
                -targetCamera.transform.forward,
                targetCamera.transform.up
            );

        // =========================================
        // 5. 화면 좌우 위치에 따라 살짝 비틀기
        // =========================================

        Quaternion sideRotation =
            Quaternion.AngleAxis(
                sideAngle,
                targetCamera.transform.up
            );

        Quaternion targetRotation =
            sideRotation *
            cameraFacingRotation;

        // =========================================
        // 6. 부드럽게 회전
        // =========================================

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }
}