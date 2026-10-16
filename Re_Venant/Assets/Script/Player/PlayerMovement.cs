using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private PlayerViewCameraController cameraController;

    [Header("References")]
    [SerializeField]
    private PlayerViewModeManager viewModeManager;

    [SerializeField]
    private PlayerAnimationController animationController;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 7f;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundCheckRadius = 0.25f;

    [SerializeField]
    private LayerMask groundMask;

    [Header("Third Person Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [SerializeField] private Camera mainCamera;
    private Rigidbody rb;

    private Vector2 moveInput;

    private bool jumpRequested;

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody>();

        if (mainCamera == null)
        {
            mainCamera =
                Camera.main;
        }

        if (cameraController == null &&
            mainCamera != null)
        {
            cameraController =
                mainCamera.GetComponent<PlayerViewCameraController>();
        }
    }

    private void Update()
    {
        ReadMovementInput();
        ReadJumpInput();

        bool isMoving =
            moveInput.sqrMagnitude > 0.01f;

        animationController?.SetMoving(
            isMoving
        );
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpRequested)
        {
            Jump();

            jumpRequested = false;
        }
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
            return;

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed)
            x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            x += 1f;

        if (Keyboard.current.sKey.isPressed)
            y -= 1f;

        if (Keyboard.current.wKey.isPressed)
            y += 1f;

        moveInput =
            new Vector2(x, y);

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    private void ReadJumpInput()
    {
        if (Keyboard.current == null)
            return;

        if (viewModeManager == null)
            return;

        // Top View에서는 점프 금지
        if (!viewModeManager.CanJump)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void Move()
    {
        if (viewModeManager == null)
            return;

        Vector3 direction = Vector3.zero;

        // =========================================
        // TOP VIEW
        // =========================================
        if (viewModeManager.CurrentMode ==
            PlayerViewMode.TopDown)
        {
            direction =
                new Vector3(
                    moveInput.x,
                    0f,
                    moveInput.y
                );

            // 회전은 PlayerMouseAim이 담당
        }

        // =========================================
        // THIRD PERSON
        // =========================================
        else if (viewModeManager.CurrentMode ==
         PlayerViewMode.ThirdPerson)
        {
            if (mainCamera == null)
                return;

            // 카메라의 앞 방향
            Vector3 cameraForward =
                mainCamera.transform.forward;

            cameraForward.y = 0f;
            cameraForward.Normalize();

            // 카메라의 오른쪽 방향
            Vector3 cameraRight =
                mainCamera.transform.right;

            cameraRight.y = 0f;
            cameraRight.Normalize();

            // 카메라 기준 WASD 이동
            direction =
                cameraForward * moveInput.y +
                cameraRight * moveInput.x;

            // ====================================
            // FREE CAMERA
            // ====================================

            bool isViewLocked =
                cameraController != null &&
                cameraController.IsThirdPersonViewLocked;

            if (!isViewLocked)
            {
                // 평소에는 이동 방향을 바라봄
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation =
                        Quaternion.LookRotation(
                            direction
                        );

                    transform.rotation =
                        Quaternion.Slerp(
                            transform.rotation,
                            targetRotation,
                            rotationSpeed *
                            Time.fixedDeltaTime
                        );
                }
            }

            // ====================================
            // LOCK CAMERA
            // ====================================

            // Lock 상태에서는 여기서 Player를
            // 이동 방향으로 돌리지 않는다.
            //
            // PlayerViewCameraController가
            // 카메라 yaw 방향으로 Player를 고정한다.
        }

        // =========================================
        // FIRST PERSON
        // =========================================
        else if (viewModeManager.CurrentMode ==
                 PlayerViewMode.FirstPerson)
        {
            // 1인칭은 Player가 이미
            // 마우스 방향을 바라보고 있으므로
            // Player 방향 기준으로 이동
            direction =
                transform.forward *
                moveInput.y +
                transform.right *
                moveInput.x;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector3 targetPosition =
            rb.position +
            direction *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(
            targetPosition
        );
    }

    private void Jump()
    {
        if (!IsGrounded())
        {
            Debug.Log("점프 실패 : Ground 감지 안됨");
            return;
        }

        Vector3 velocity =
            rb.linearVelocity;

        // 기존 Y 속도 제거
        velocity.y = 0f;

        rb.linearVelocity =
            velocity;

        // 위쪽으로 순간 속도 부여
        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.VelocityChange
        );

        Debug.Log("JUMP!");
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}