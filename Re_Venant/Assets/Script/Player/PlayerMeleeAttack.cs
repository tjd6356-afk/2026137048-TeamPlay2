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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bryySound;
    [SerializeField] private AudioClip yahaSound;

    [Range(0f, 1f)]
    [SerializeField] private float attackSoundVolume = 1f;

    private int attackSoundIndex = 0;

    private bool isAttacking = false;

    private Quaternion weaponStartRotation;

    private void Start()
    {
        if (weaponTransform != null)
        {
            weaponStartRotation =
                weaponTransform.localRotation;
        }
    }

    private void Update()
    {
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

        // 공격 음성 재생
        PlayAttackSound();

        // 실제 공격 실행
        StartCoroutine(AttackRoutine());
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
        if (audioSource == null)
            return;

        AudioClip soundToPlay = null;

        if (attackSoundIndex == 0)
        {
            soundToPlay = bryySound;
            attackSoundIndex = 1;
        }
        else
        {
            soundToPlay = yahaSound;
            attackSoundIndex = 0;
        }

        if (soundToPlay != null)
        {
            audioSource.PlayOneShot(
                soundToPlay,
                attackSoundVolume
            );
        }
    }
}