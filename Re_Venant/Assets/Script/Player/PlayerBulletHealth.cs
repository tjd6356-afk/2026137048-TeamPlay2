using System;
using UnityEngine;

public class PlayerBulletHealth : MonoBehaviour
{
    [Header("Bullet Health")]
    [SerializeField] private int maxBullets = 100;

    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager;

    private int currentBullets;
    private bool isDead = false;

    public int MaxBullets => maxBullets;
    public int CurrentBullets => currentBullets;
    public bool IsDead => isDead;

    // 현재 총알, 최대 총알
    public event Action<int, int> OnBulletsChanged;

    private void Awake()
    {
        currentBullets = maxBullets;
    }

    private void Start()
    {
        NotifyBulletsChanged();
    }

    // 총 발사 시 사용
    public bool UseBullet(int amount = 1)
    {
        if (isDead)
            return false;

        if (amount <= 0)
            return false;

        if (currentBullets < amount)
            return false;

        currentBullets -= amount;

        currentBullets = Mathf.Clamp(
            currentBullets,
            0,
            maxBullets
        );

        NotifyBulletsChanged();

        Debug.Log(
            $"Bullet : {currentBullets} / {maxBullets}"
        );

        CheckDeath();

        return true;
    }

    // 적에게 공격받았을 때
    public void TakeDamage(int bulletLoss)
    {
        if (isDead)
            return;

        if (bulletLoss <= 0)
            return;

        currentBullets -= bulletLoss;

        currentBullets = Mathf.Clamp(
            currentBullets,
            0,
            maxBullets
        );

        NotifyBulletsChanged();

        Debug.Log(
            $"Player Hit! Bullet -{bulletLoss} / {currentBullets} / {maxBullets}"
        );

        CheckDeath();
    }

    // 총알 회복
    public void AddBullets(int amount)
    {
        if (isDead)
            return;

        if (amount <= 0)
            return;

        currentBullets += amount;

        currentBullets = Mathf.Clamp(
            currentBullets,
            0,
            maxBullets
        );

        NotifyBulletsChanged();
    }

    private void NotifyBulletsChanged()
    {
        OnBulletsChanged?.Invoke(
            currentBullets,
            maxBullets
        );
    }

    private void CheckDeath()
    {
        if (currentBullets > 0)
            return;

        Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player Dead - Bullet Empty");

        if (gameOverManager != null)
        {
            gameOverManager.GameOver();
        }

        Destroy(gameObject);
    }
}