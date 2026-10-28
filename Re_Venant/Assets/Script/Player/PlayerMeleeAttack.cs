using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Transform weaponTransform;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Transform hitboxPoint;

    [Header("Attack")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 0.6f;

    [Header("Hitbox Time")]
    [SerializeField] private float hitboxLifeTime = 0.15f;
    [SerializeField] private float hitboxSpawnDelay = 0.08f;

    [Header("Weapon Swing")]
    [SerializeField] private float swingAngle = -120f;
    [SerializeField] private float swingDuration = 0.25f;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    [Range(0f, 1f)]
    [SerializeField] private float attackSoundVolume = 1f;

    [SerializeField] private float soundGap = 0f;

    private float meleeRangeMultiplier = 1f;

    private Vector3 baseWeaponScale;
    private Vector3 baseHitboxPointPosition;

    private bool isAttacking = false;

    private Quaternion weaponStartRotation;

    private void Start()
    {
        if (weaponTransform != null)
        {
            weaponStartRotation =
                weaponTransform.localRotation;

            baseWeaponScale =
                weaponTransform.localScale;
        }

        if (hitboxPoint != null)
        {
            baseHitboxPointPosition =
                hitboxPoint.localPosition;
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        if (Mouse.current == null)
            return;

        // 우클릭
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isAttacking)
            return;

        if (weaponTransform == null)
            return;

        if (hitboxPrefab == null)
            return;

        if (hitboxPoint == null)
            return;

        // 근접 공격 효과음
        PlayAttackSound();

        // 공격 실행
        StartCoroutine(
            AttackRoutine()
        );
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        bool hitboxSpawned = false;

        Quaternion originalRotation =
            weaponStartRotation;

        // 오른쪽 시작 위치
        Quaternion rightRotation =
            weaponStartRotation *
            Quaternion.Euler(
                0f,
                -swingAngle * 0.5f,
                0f
            );

        // 왼쪽 끝 위치
        Quaternion leftRotation =
            weaponStartRotation *
            Quaternion.Euler(
                0f,
                swingAngle * 0.5f,
                0f
            );

        // 공격 시작 전에 무기를 오른쪽으로 이동
        weaponTransform.localRotation =
            rightRotation;

        float timer = 0f;

        // 오른쪽 → 왼쪽으로 휘두르기
        while (timer < swingDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / swingDuration
                );

            weaponTransform.localRotation =
                Quaternion.Slerp(
                    rightRotation,
                    leftRotation,
                    t
                );

            // 휘두르는 도중 히트박스 생성
            if (!hitboxSpawned &&
                timer >= hitboxSpawnDelay)
            {
                SpawnHitbox();

                hitboxSpawned = true;
            }

            yield return null;
        }

        // 공격 종료 후 원래 자세로 복귀
        timer = 0f;

        float returnDuration = 0.1f;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / returnDuration
                );

            weaponTransform.localRotation =
                Quaternion.Slerp(
                    leftRotation,
                    originalRotation,
                    t
                );

            yield return null;
        }

        weaponTransform.localRotation =
            originalRotation;

        float remainingCooldown =
            attackCooldown -
            swingDuration -
            returnDuration;

        if (remainingCooldown > 0f)
        {
            yield return new WaitForSeconds(
                remainingCooldown
            );
        }

        isAttacking = false;
    }

    private void SpawnHitbox()
    {
        GameObject hitboxObject =
            Instantiate(
                hitboxPrefab,
                hitboxPoint.position,
                hitboxPoint.rotation,
                hitboxPoint
            );

        // 강화된 근접 공격 범위 적용
        hitboxObject.transform.localScale *=
            meleeRangeMultiplier;

        MeleeHitbox hitbox =
            hitboxObject.GetComponent<MeleeHitbox>();

        if (hitbox != null)
        {
            hitbox.Initialize(
                damage,
                hitboxLifeTime,
                transform
            );
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound == null)
            return;

        if (GameAudioManager.Instance == null)
            return;

        GameAudioManager.Instance.PlaySFX(
            attackSound,
            attackSoundVolume
        );
    }

    public void IncreaseDamage(float amount)
    {
        if (amount <= 0f)
            return;

        damage += amount;

        Debug.Log(
            $"Melee Damage 증가 : {damage}"
        );
    }

    public void IncreaseRange(float amount)
    {
        if (amount <= 0f)
            return;

        meleeRangeMultiplier += amount;

        // 실제 Weapon 크기도 증가
        if (weaponTransform != null)
        {
            weaponTransform.localScale =
                baseWeaponScale *
                meleeRangeMultiplier;
        }

        // Hitbox 생성 지점도 조금 더 앞으로 이동
        if (hitboxPoint != null)
        {
            Vector3 newPosition =
                baseHitboxPointPosition;

            newPosition.z =
                baseHitboxPointPosition.z *
                meleeRangeMultiplier;

            hitboxPoint.localPosition =
                newPosition;
        }

        Debug.Log(
            $"Melee Range 증가 : x{meleeRangeMultiplier}"
        );
    }
}