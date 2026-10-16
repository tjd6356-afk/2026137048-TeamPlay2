using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Bullet Health")]
    [SerializeField] private PlayerBulletHealth bulletHealth;

    [Header("Shoot")]
    [SerializeField] private int projectilesPerShot = 1;

    [Tooltip("초당 발사 횟수")]
    [SerializeField] private float fireRate = 2f;

    [Tooltip("여러 발 발사 시 탄환 사이 각도")]
    [SerializeField] private float spreadAngle = 8f;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    [Range(0f, 1f)]
    [SerializeField] private float shootVolume = 1f;

    [Header("View")]
    [SerializeField]
    private PlayerViewModeManager viewModeManager;

    [Header("Aim")]
    [SerializeField]
    private LayerMask aimLayerMask = ~0;

    [SerializeField]
    private float aimDistance = 200f;
    private Camera mainCam;

    private float nextFireTime;

    public int ProjectilesPerShot => projectilesPerShot;
    public float FireRate => fireRate;

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

        if (bulletHealth == null)
            return;

        if (bulletHealth.IsDead)
            return;

        // 마우스를 누르고 있으면 발사속도에 맞춰 계속 발사
        if (Mouse.current.leftButton.isPressed &&
            Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 shootDirection;

        if (!TryGetShootDirection(
            out shootDirection))
        {
            return;
        }

        if (mainCam == null)
            return;

        if (bulletPrefab == null)
            return;

        if (firePoint == null)
            return;

        if (bulletHealth.CurrentBullets <= 0)
            return;

        // 남은 총알보다 많이 발사하지 않음
        int bulletsToFire =
            Mathf.Min(
                projectilesPerShot,
                bulletHealth.CurrentBullets
            );

        // 여러 발을 부채꼴로 발사
        float totalSpread =
            spreadAngle *
            (bulletsToFire - 1);

        float startAngle =
            -totalSpread * 0.5f;

        for (int i = 0; i < bulletsToFire; i++)
        {
            float angle =
                startAngle +
                spreadAngle * i;

            Vector3 direction =
                Quaternion.AngleAxis(
                    angle,
                    Vector3.up
                ) * shootDirection;

            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(direction)
            );
        }

        // 실제 발사된 탄환 개수만큼 체력/탄환 소비
        bulletHealth.UseBullet(
            bulletsToFire
        );

        if (animationController != null)
        {
            animationController.PlayThrow();
        }

        if (audioSource != null &&
            shootSound != null)
        {
            audioSource.PlayOneShot(
                shootSound,
                shootVolume
            );
        }

        nextFireTime =
            Time.time +
            (1f / fireRate);
    }

    // ----------------------------
    // 강화 카드에서 사용하는 함수
    // ----------------------------

    public void IncreaseProjectilesPerShot(int amount)
    {
        if (amount <= 0)
            return;

        projectilesPerShot += amount;

        projectilesPerShot =
            Mathf.Max(
                1,
                projectilesPerShot
            );

        Debug.Log(
            $"발사량 증가 : {projectilesPerShot}"
        );
    }

    public void IncreaseFireRate(float amount)
    {
        if (amount <= 0f)
            return;

        fireRate += amount;

        fireRate =
            Mathf.Max(
                0.1f,
                fireRate
            );

        Debug.Log(
            $"발사속도 증가 : {fireRate}"
        );
    }

    private bool TryGetShootDirection(
    out Vector3 shootDirection)
    {
        shootDirection =
            Vector3.zero;

        if (mainCam == null ||
            firePoint == null)
        {
            return false;
        }

        // ==================================
        // TOP VIEW
        // ==================================

        if (viewModeManager == null ||
            viewModeManager.CurrentMode ==
            PlayerViewMode.TopDown)
        {
            if (Mouse.current == null)
                return false;

            Vector2 mousePosition =
                Mouse.current.position.ReadValue();

            Ray ray =
                mainCam.ScreenPointToRay(
                    mousePosition
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

            if (!groundPlane.Raycast(
                ray,
                out float enter))
            {
                return false;
            }

            Vector3 hitPoint =
                ray.GetPoint(enter);

            shootDirection =
                hitPoint -
                firePoint.position;

            shootDirection.y = 0f;
        }

        // ==================================
        // THIRD / FIRST PERSON
        // ==================================

        else
        {
            Vector2 screenCenter =
                new Vector2(
                    Screen.width * 0.5f,
                    Screen.height * 0.5f
                );

            Ray ray =
                mainCam.ScreenPointToRay(
                    screenCenter
                );

            Vector3 targetPoint;

            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                aimDistance,
                aimLayerMask,
                QueryTriggerInteraction.Ignore))
            {
                targetPoint =
                    hit.point;
            }
            else
            {
                targetPoint =
                    ray.origin +
                    ray.direction *
                    aimDistance;
            }

            shootDirection =
                targetPoint -
                firePoint.position;
        }

        if (shootDirection.sqrMagnitude <
            0.001f)
        {
            return false;
        }

        shootDirection.Normalize();

        return true;
    }
}