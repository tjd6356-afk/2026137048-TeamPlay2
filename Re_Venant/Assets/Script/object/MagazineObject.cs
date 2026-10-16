using UnityEngine;

public class MagazineObject : MonoBehaviour
{
    [Header("Magazine Health")]
    [SerializeField, Tooltip("탄창 오브젝트의 체력 (몇 데미지를 받아야 부서질지)")]
    private float maxHealth = 20f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Ammo Reward")]
    [SerializeField, Tooltip("파괴 시 회복될 총알(체력) 수치")]
    private int ammoRewardAmount = 30;

    // 플레이어의 상태를 관리하는 스크립트 참조
    private PlayerBulletHealth playerHealth;

    private void Awake()
    {
        // 생성 시 체력을 최대로 초기화
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // 맵에 있는 플레이어의 PlayerBulletHealth 스크립트를 찾아 캐싱합니다.
        playerHealth = FindFirstObjectByType<PlayerBulletHealth>();
    }

    // 플레이어의 총알(투사체)이 이 오브젝트를 맞췄을 때 호출할 함수
    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Magazine Damaged : {damage} / HP : {currentHealth} / {maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Magazine Destroyed!");

        // 1. 탄창 파괴 시 플레이어의 총알(체력) 회복
        if (playerHealth != null)
        {
            playerHealth.AddBullets(ammoRewardAmount);
            Debug.Log($"플레이어 총알 {ammoRewardAmount} 회복됨!");
        }
        else
        {
            Debug.LogWarning("PlayerBulletHealth를 찾을 수 없어 총알을 회복할 수 없습니다.");
        }

        // (선택) 여기에 파괴 파티클 이펙트나 효과음 재생 코드를 추가할 수 있습니다.

        // 2. 오브젝트 파괴
        // 파괴되는 즉시 MagazineSpawner의 목록에서 null로 처리되어 자동으로 스폰 타이머가 재개됩니다.
        Destroy(gameObject);
    }
}