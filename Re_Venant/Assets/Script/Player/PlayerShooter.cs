using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Bullet Health")]
    [SerializeField] private PlayerBulletHealth bulletHealth;

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

        if (bulletHealth == null)
        {
            bulletHealth =
                GetComponent<PlayerBulletHealth>();
        }

        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

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

        if (bulletHealth == null)
            return;

        // 이미 죽었으면 발사 불가
        if (bulletHealth.IsDead)
            return;

        // 남은 총알이 없으면 발사 불가
        if (bulletHealth.CurrentBullets <= 0)
            return;

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCam.ScreenPointToRay(
                mouseScreenPosition
            );

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

            if (shootDirection.sqrMagnitude < 0.001f)
                return;

            shootDirection.Normalize();

            // 총알 생성
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(
                    shootDirection
                )
            );

            // 발사했으므로 총알 1개 소비
            bulletHealth.UseBullet(1);

            // Throw 애니메이션
            if (animationController != null)
            {
                animationController.PlayThrow();
            }

            // 발사 효과음
            if (audioSource != null &&
                shootSound != null)
            {
                audioSource.PlayOneShot(
                    shootSound,
                    shootVolume
                );
            }
        }
    }
}