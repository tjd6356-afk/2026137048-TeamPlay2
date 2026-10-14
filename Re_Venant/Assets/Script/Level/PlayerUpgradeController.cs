using UnityEngine;

public class PlayerUpgradeController : MonoBehaviour
{
    [Header("Player Systems")]
    [SerializeField] private PlayerBulletHealth bulletHealth;
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private PlayerMeleeAttack meleeAttack;

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
        }

        Debug.Log(
            $"Upgrade 획득 : {card.cardName}"
        );
    }
}