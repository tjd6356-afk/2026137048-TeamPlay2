using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button button;

    private UpgradeCardData currentCard;
    private LevelUpManager levelUpManager;

    public void Setup(
        UpgradeCardData card,
        LevelUpManager manager)
    {
        currentCard = card;
        levelUpManager = manager;

        if (titleText != null)
        {
            titleText.text =
                card.cardName;
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                card.description;
        }

        if (iconImage != null)
        {
            iconImage.sprite =
                card.icon;

            iconImage.enabled =
                card.icon != null;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                SelectCard
            );
        }
    }

    private void SelectCard()
    {
        if (currentCard == null)
            return;

        if (levelUpManager == null)
            return;

        levelUpManager.SelectCard(
            currentCard
        );
    }
}