using UnityEngine;
using UnityEngine.InputSystem;

public class ProtectHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager;

    private float currentHealth;
    private bool isDead = false;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            TakeDamage(1000f);
        }
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
            $"Protect Damage : {damage} / HP : {currentHealth} / {maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Protect Destroyed");

        // 게임 오버 실행
        if (gameOverManager != null)
        {
            gameOverManager.GameOver();
        }

        // Protect 오브젝트 삭제
        Destroy(gameObject);
    }
}