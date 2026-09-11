using UnityEngine;
using UnityEngine.InputSystem;

public class EmployeeClickSelector : MonoBehaviour
{
    [Header("카메라")]
    public Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        TrySelectEmployee();
    }

    private void TrySelectEmployee()
    {
        if (mainCamera == null)
            return;

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    0f
                )
            );

        Vector2 point =
            new Vector2(
                worldPosition.x,
                worldPosition.y
            );

        // 겹쳐 있는 Collider가 있을 수도 있으므로 전부 검사
        Collider2D[] hits =
            Physics2D.OverlapPointAll(point);

        foreach (Collider2D hit in hits)
        {
            EmployeeView employee =
                hit.GetComponent<EmployeeView>();

            if (employee == null)
                continue;

            PrototypeGameManager.Instance
                .SelectEmployee(employee.EmployeeId);

            Debug.Log(
                $"직원 클릭 성공 : {employee.EmployeeId}"
            );

            return;
        }
    }
}