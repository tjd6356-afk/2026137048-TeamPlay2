using UnityEngine;

public class PlayerUpgradeController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LevelUpManager levelUpManager;

    [Header("Player Systems")]
    [SerializeField] private PlayerBulletHealth bulletHealth;
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private PlayerMeleeAttack meleeAttack;

    [Header("Experience Orb Upgrade")]
    [SerializeField] private float experienceOrbMultiplier = 1f;

    public float ExperienceOrbMultiplier =>
        experienceOrbMultiplier;

    private void Awake()
    {
        if (bulletHealth == null)
        {
            bulletHealth =
                GetComponent<PlayerBulletHealth>();
        }

        if (shooter == null)
        {
            shooter =
                GetComponent<PlayerShooter>();
        }

        if (meleeAttack == null)
        {
            meleeAttack =
                GetComponent<PlayerMeleeAttack>();
        }

        if (scoreManager == null)
        {
            scoreManager =
                FindFirstObjectByType<ScoreManager>();
        }

        if (levelUpManager == null)
        {
            levelUpManager =
                FindFirstObjectByType<LevelUpManager>();
        }
    }

    public void ApplyUpgrade(
        UpgradeCardData card)
    {
        if (card == null)
            return;

        switch (card.upgradeType)
        {
            case UpgradeType.MaxBullets:

                if (bulletHealth != null)
                {
                    bulletHealth.IncreaseMaxBullets(
                        card.intValue
                    );
                }

                break;


            case UpgradeType.ProjectilesPerShot:

                if (shooter != null)
                {
                    shooter.IncreaseProjectilesPerShot(
                        card.intValue
                    );
                }

                break;


            case UpgradeType.MeleeDamage:

                if (meleeAttack != null)
                {
                    meleeAttack.IncreaseDamage(
                        card.floatValue
                    );
                }

                break;


            case UpgradeType.MeleeRange:

                if (meleeAttack != null)
                {
                    meleeAttack.IncreaseRange(
                        card.floatValue
                    );
                }

                break;


            case UpgradeType.FireRate:

                if (shooter != null)
                {
                    shooter.IncreaseFireRate(
                        card.floatValue
                    );
                }

                break;

            case UpgradeType.ExperienceOrbAmount:

                IncreaseExperienceOrbAmount(
                    card.floatValue
                );

                break;


            case UpgradeType.ScoreMultiplier:

                if (scoreManager != null)
                {
                    scoreManager.IncreaseScoreMultiplier(
                        card.floatValue
                    );
                }

                break;


            case UpgradeType.FreeReroll:

                if (levelUpManager != null)
                {
                    levelUpManager.IncreaseFreeRerollsPerLevel(
                        card.intValue
                    );
                }

                break;
        }

        Debug.Log(
            $"Upgrade 획득 : {card.cardName}"
        );
    }

    public void IncreaseExperienceOrbAmount(
    float multiplierIncrease)
    {
        if (multiplierIncrease <= 0f)
            return;

        experienceOrbMultiplier +=
            multiplierIncrease;

        Debug.Log(
            $"경험치 Orb 배율 증가 : x{experienceOrbMultiplier}"
        );
    }

    public int GetModifiedExperienceAmount(
    int baseAmount)
    {
        if (baseAmount <= 0)
            return 0;

        return Mathf.Max(
            1,
            Mathf.RoundToInt(
                baseAmount *
                experienceOrbMultiplier
            )
        );
    }
}