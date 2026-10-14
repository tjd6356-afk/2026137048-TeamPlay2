using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerExperience playerExperience;

    [Header("UI")]
    [SerializeField] private Image expFillImage;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        if (playerExperience == null)
        {
            playerExperience =
                FindFirstObjectByType<PlayerExperience>();
        }

        if (playerExperience == null)
            return;

        playerExperience.OnExpChanged += UpdateExpUI;
        playerExperience.OnLevelChanged += UpdateLevelUI;

        UpdateExpUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );

        UpdateLevelUI(
            playerExperience.CurrentLevel
        );
    }

    private void OnDestroy()
    {
        if (playerExperience == null)
            return;

        playerExperience.OnExpChanged -= UpdateExpUI;
        playerExperience.OnLevelChanged -= UpdateLevelUI;
    }

    private void UpdateExpUI(
        int currentExp,
        int requiredExp)
    {
        float ratio = 0f;

        if (requiredExp > 0)
        {
            ratio =
                (float)currentExp /
                requiredExp;
        }

        if (expFillImage != null)
        {
            expFillImage.fillAmount =
                Mathf.Clamp01(ratio);
        }

        if (expText != null)
        {
            expText.text =
                $"{currentExp} / {requiredExp}";
        }
    }

    private void UpdateLevelUI(int level)
    {
        if (levelText != null)
        {
            levelText.text =
                $"LV. {level}";
        }
    }
}