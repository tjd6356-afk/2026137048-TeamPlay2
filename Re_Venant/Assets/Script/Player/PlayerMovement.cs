using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
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

    private Rigidbody rb;

    private Vector2 moveInput;

    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        Vector3 direction;

        if (viewModeManager != null &&
            viewModeManager.CurrentMode !=
            PlayerViewMode.TopDown)
        {
            // 3인칭 / 1인칭
            // 캐릭터가 바라보는 방향 기준 이동
            direction =
                transform.right *
                moveInput.x +
                transform.forward *
                moveInput.y;
        }
        else
        {
            // 기존 Top View
            direction =
                new Vector3(
                    moveInput.x,
                    0f,
                    moveInput.y
                );
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