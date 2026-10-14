using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BulletUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerBulletHealth bulletHealth;

    [Header("UI")]
    [SerializeField] private Image bulletFillImage;
    [SerializeField] private TMP_Text bulletText;

    private void Start()
    {
        if (bulletHealth == null)
        {
            bulletHealth =
                FindFirstObjectByType<PlayerBulletHealth>();
        }

        if (bulletHealth == null)
        {
            Debug.LogWarning(
                "PlayerBulletHealth를 찾지 못했습니다."
            );

            return;
        }

        bulletHealth.OnBulletsChanged += UpdateUI;

        // 시작하자마자 한번 갱신
        UpdateUI(
            bulletHealth.CurrentBullets,
            bulletHealth.MaxBullets
        );
    }

    private void OnDestroy()
    {
        if (bulletHealth != null)
        {
            bulletHealth.OnBulletsChanged -= UpdateUI;
        }
    }

    private void UpdateUI(
        int currentBullets,
        int maxBullets)
    {
        float ratio = 0f;

        if (maxBullets > 0)
        {
            ratio =
                (float)currentBullets /
                maxBullets;
        }

        if (bulletFillImage != null)
        {
            bulletFillImage.fillAmount =
                Mathf.Clamp01(ratio);
        }

        if (bulletText != null)
        {
            bulletText.text =
                $"{currentBullets} / {maxBullets}";
        }
    }
}