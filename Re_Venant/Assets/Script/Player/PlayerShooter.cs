using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [Range(0f, 1f)]
    [SerializeField] private float shootVolume = 1f;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        // 직접 넣지 않았으면 Player에서 자동으로 찾기
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
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

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCam.ScreenPointToRay(mouseScreenPosition);

        Plane groundPlane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    firePoint.position.y,
                    0f
                )
            );

        if (groundPlane.Raycast(ray, out float enterDistance))
        {
            Vector3 hitPoint =
                ray.GetPoint(enterDistance);

            Vector3 shootDirection =
                hitPoint - firePoint.position;

            shootDirection.y = 0f;

            if (shootDirection.sqrMagnitude < 0.001f)
                return;

            shootDirection.Normalize();

            // 총알 생성
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(shootDirection)
            );

            // 공격 애니메이션
            if (animationController != null)
            {
                animationController.PlayThrow();
            }

            // 발사 효과음
            PlayShootSound();
        }
    }

    private void PlayShootSound()
    {
        if (audioSource == null)
            return;

        if (shootSound == null)
            return;

        audioSource.PlayOneShot(
            shootSound,
            shootVolume
        );
    }
}