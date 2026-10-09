using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController animationController;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // 마우스 왼쪽 버튼을 누른 순간 발사
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (mainCam == null)
            return;

        if (bulletPrefab == null)
            return;

        if (firePoint == null)
            return;

        // 마우스 화면 좌표
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        // 카메라 -> 마우스 방향 Ray
        Ray ray =
            mainCam.ScreenPointToRay(mouseScreenPosition);

        // FirePoint와 같은 높이의 가상 평면
        Plane groundPlane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    firePoint.position.y,
                    0f
                )
            );

        if (groundPlane.Raycast(
            ray,
            out float enterDistance))
        {
            Vector3 hitPoint =
                ray.GetPoint(enterDistance);

            Vector3 shootDirection =
                hitPoint - firePoint.position;

            shootDirection.y = 0f;

            // 방향값이 너무 작으면 발사하지 않음
            if (shootDirection.sqrMagnitude < 0.001f)
                return;

            shootDirection.Normalize();

            // 총알 생성
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(shootDirection)
            );

            // 총알이 실제로 발사된 순간 Throw 애니메이션 실행
            if (animationController != null)
            {
                animationController.PlayThrow();
            }
        }
    }
}