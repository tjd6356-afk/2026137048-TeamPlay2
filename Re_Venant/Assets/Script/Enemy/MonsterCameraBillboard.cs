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
        // 1. 화면에서 몬스터의 좌우 위치 계산
        // =========================================

        Vector3 viewportPosition =
            targetCamera.WorldToViewportPoint(
                transform.position
            );

        // 왼쪽  = -1
        // 중앙  =  0
        // 오른쪽 = +1

        float horizontalPosition =
            (viewportPosition.x - 0.5f) * 2f;

        horizontalPosition =
            Mathf.Clamp(
                horizontalPosition,
                -1f,
                1f
            );

        // =========================================
        // 2. 중앙 영역에서는 완전 정면
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
        // 3. 화면 좌우에 따른 추가 Y 회전
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
        // 4. 카메라 방향에서 위/아래 회전 제거
        // =========================================

        Vector3 flatCameraForward =
            targetCamera.transform.forward;

        // 핵심:
        // 카메라의 위/아래 방향을 무시
        flatCameraForward.y = 0f;

        if (flatCameraForward.sqrMagnitude < 0.001f)
            return;

        flatCameraForward.Normalize();

        // World Space Canvas가
        // 카메라 방향을 바라보게 함.
        // 단, Y축으로만 회전.
        Quaternion baseRotation =
            Quaternion.LookRotation(
                -flatCameraForward,
                Vector3.up
            );

        // =========================================
        // 5. 좌우 몬스터만 약간 비스듬하게
        // =========================================

        Quaternion sideRotation =
            Quaternion.Euler(
                0f,
                sideAngle,
                0f
            );

        Quaternion targetRotation =
            baseRotation *
            sideRotation;

        // =========================================
        // 6. 부드럽게 회전
        // =========================================

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }
}