using UnityEngine;
using UnityEngine.InputSystem; // 새로운 Input System 사용

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab; // 발사할 탄환 프리팹
    [SerializeField] private Transform firePoint;     // 탄환이 생성될 위치

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // 마우스 입력을 받을 수 없는 상태면 리턴
        if (Mouse.current == null) return;

        // 마우스 왼쪽 버튼을 누른 순간
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 1. 현재 마우스의 화면(Screen) 좌표를 가져옵니다.
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // 2. 카메라에서 마우스 위치를 향해 Ray를 쏩니다.
        Ray ray = mainCam.ScreenPointToRay(mouseScreenPosition);

        // 3. 플레이어의 총구(firePoint) 높이와 동일한 높이를 가진 가상의 평면을 만듭니다.
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, firePoint.position.y, 0));

        // 4. Ray가 평면과 부딪혔는지 검사합니다.
        if (groundPlane.Raycast(ray, out float enterDistance))
        {
            // Ray가 평면과 부딪힌 정확한 3D 월드 좌표
            Vector3 hitPoint = ray.GetPoint(enterDistance);

            // 5. 누른 '방향' 계산 (목표지점 - 시작지점)
            Vector3 shootDirection = hitPoint - firePoint.position;
            shootDirection.y = 0f; // 위아래로 발사되지 않도록 y축 고정
            shootDirection.Normalize(); // 방향만 남기기 위해 정규화

            // 6. 총알 생성 및 방향 설정
            // 탄환이 날아갈 방향을 바라보게(Rotation) 생성합니다.
            Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        }
    }
}