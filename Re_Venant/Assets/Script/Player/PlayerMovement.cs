using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal += 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical -= 1f;

        if (Keyboard.current.wKey.isPressed)
            vertical += 1f;

        moveInput = new Vector2(horizontal, vertical);

        // 대각선 이동 속도 증가 방지
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        Vector3 targetPosition =
            rb.position +
            moveDirection * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }
}