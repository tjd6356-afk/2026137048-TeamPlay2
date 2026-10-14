using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Score")]
    [SerializeField] private int killScore = 100;

    private float currentHealth;
    private bool isDead = false;

    private ScoreManager scoreManager;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        scoreManager =
            FindFirstObjectByType<ScoreManager>();
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

        // 적 처치 점수 지급
        if (scoreManager != null)
        {
            scoreManager.AddKillScore(
                killScore
            );
        }

        Destroy(gameObject);
    }
}