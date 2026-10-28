using UnityEngine;

public class MonsterCameraBillboard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [SerializeField]
    private PlayerViewModeManager viewModeManager;

    [Header("Third / First Person")]
    [Range(0f, 0.5f)]
    [SerializeField] private float centerDeadZone = 0.12f;

    [Range(0f, 60f)]
    [SerializeField] private float maxSideAngle = 30f;

    [SerializeField] private bool invertSideAngle = false;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Top View")]
    [Tooltip("Top View에서 이미지가 거꾸로 보이면 180으로 바꿔보세요.")]
    [SerializeField] private float topViewRotationOffset = 0f;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (viewModeManager == null)
        {
            viewModeManager =
                FindFirstObjectByType<PlayerViewModeManager>();
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        // ViewModeManager가 없으면
        // 기존 3D 방식으로 처리
        if (viewModeManager == null)
        {
            UpdateThirdFirstPersonFacing();
            return;
        }

        switch (viewModeManager.CurrentMode)
        {
            case PlayerViewMode.TopDown:

                UpdateTopViewFacing();

                break;


            case PlayerViewMode.ThirdPerson:
            case PlayerViewMode.FirstPerson:

                UpdateThirdFirstPersonFacing();

                break;
        }
    }

    // ==================================================
    // TOP VIEW
    // ==================================================

    private void UpdateTopViewFacing()
    {
        if (targetCamera == null)
            return;

        // Top View에서는 몬스터 위치와 관계없이
        // 모든 Canvas가 똑같이 카메라 화면을 바라봄.
        Quaternion targetRotation =
            Quaternion.LookRotation(
                -targetCamera.transform.forward,
                targetCamera.transform.up
            );

        // 필요하면 이미지 위/아래 방향만 보정
        targetRotation *=
            Quaternion.Euler(
                0f,
                0f,
                topViewRotationOffset
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    // ==================================================
    // THIRD PERSON / FIRST PERSON
    // ==================================================

    private void UpdateThirdFirstPersonFacing()
    {
        // ------------------------------------------
        // 화면에서 몬스터 좌우 위치 계산
        // ------------------------------------------

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

        // ------------------------------------------
        // 중앙에서는 정면
        // ------------------------------------------

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

        // ------------------------------------------
        // 화면 좌우에 따른 비스듬한 각도
        // ------------------------------------------

        float sideAngle =
            horizontalPosition *
            maxSideAngle *
            sideStrength;

        if (invertSideAngle)
        {
            sideAngle *= -1f;
        }

        // ------------------------------------------
        // 카메라의 위/아래 각도는 무시
        // ------------------------------------------

        Vector3 flatCameraForward =
            targetCamera.transform.forward;

        flatCameraForward.y = 0f;

        if (flatCameraForward.sqrMagnitude < 0.001f)
            return;

        flatCameraForward.Normalize();

        Quaternion baseRotation =
            Quaternion.LookRotation(
                -flatCameraForward,
                Vector3.up
            );

        Quaternion sideRotation =
            Quaternion.Euler(
                0f,
                sideAngle,
                0f
            );

        Quaternion targetRotation =
            baseRotation *
            sideRotation;

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }
}