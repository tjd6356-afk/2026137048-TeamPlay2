using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Score")]
    [SerializeField] private int killScore = 100;

    [Header("Experience Drop")]
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private int experienceAmount = 3;

    [Header("Hit Visual")]
    [SerializeField] private MonsterHitVisual hitVisual;

    private PlayerUpgradeController upgradeController;
    private float currentHealth;
    private bool isDead = false;

    private ScoreManager scoreManager;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (hitVisual == null)
        {
            hitVisual =
                GetComponentInChildren<MonsterHitVisual>();
        }
    }

    private void Start()
    {
        scoreManager =
            FindFirstObjectByType<ScoreManager>();

        upgradeController =
            FindFirstObjectByType<PlayerUpgradeController>();
    }

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
            $"Enemy Damage : {damage} / HP : {currentHealth} / {maxHealth}"
        );

        // 살아남은 공격이면 피격 연출 실행
        if (currentHealth > 0f)
        {
            if (hitVisual != null)
            {
                hitVisual.PlayHitEffect();
            }
        }

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

        Debug.Log("Enemy Dead");

        // 점수 지급
        if (scoreManager != null)
        {
            scoreManager.AddKillScore(
                killScore
            );
        }

        // 경험치 드롭
        DropExperience();

        Destroy(gameObject);
    }

    private void DropExperience()
    {
        if (experiencePrefab == null)
            return;

        int finalExperienceAmount =
            experienceAmount;

        // 경험치 Orb 강화 카드 적용
        if (upgradeController != null)
        {
            finalExperienceAmount =
                upgradeController.GetModifiedExperienceAmount(
                    experienceAmount
                );
        }

        GameObject expObject =
            Instantiate(
                experiencePrefab,
                transform.position,
                Quaternion.identity
            );

        ExperienceOrb expOrb =
            expObject.GetComponent<ExperienceOrb>();

        if (expOrb != null)
        {
            expOrb.SetExperience(
                finalExperienceAmount
            );
        }

        Debug.Log(
            $"EXP Orb Drop : {finalExperienceAmount}"
        );
    }
}