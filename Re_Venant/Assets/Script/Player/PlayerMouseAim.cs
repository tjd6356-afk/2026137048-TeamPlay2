using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseAim : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera mainCamera;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 20f;

    public Vector3 MouseWorldPosition { get; private set; }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        if (mainCamera == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(
            mouseScreenPosition
        );

        // 플레이어 높이와 같은 수평면 생성
        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(
                0f,
                transform.position.y,
                0f
            )
        );

        if (!groundPlane.Raycast(ray, out float enter))
            return;

        Vector3 mouseWorldPosition =
            ray.GetPoint(enter);

        MouseWorldPosition = mouseWorldPosition;

        Vector3 direction =
            mouseWorldPosition - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}