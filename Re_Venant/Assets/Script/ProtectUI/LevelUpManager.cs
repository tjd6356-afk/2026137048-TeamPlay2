using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private PlayerUpgradeController upgradeController;

    [Header("UI")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("Cards")]
    [SerializeField]
    private List<UpgradeCardData> availableCards =
        new List<UpgradeCardData>();

    [SerializeField]
    private List<LevelUpCardUI> cardSlots =
        new List<LevelUpCardUI>();

    private bool isLevelUpOpen = false;

    // 한번에 여러 레벨업을 했을 때 저장
    private int pendingLevelUps = 0;

    private void Start()
    {
        if (playerExperience == null)
        {
            playerExperience =
                FindFirstObjectByType<PlayerExperience>();
        }

        if (upgradeController == null)
        {
            upgradeController =
                FindFirstObjectByType<PlayerUpgradeController>();
        }

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        if (playerExperience != null)
        {
            playerExperience.OnLevelUp +=
                OnPlayerLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp -=
                OnPlayerLevelUp;
        }
    }

    private void OnPlayerLevelUp()
    {
        pendingLevelUps++;

        if (!isLevelUpOpen)
        {
            OpenLevelUpPanel();
        }
    }

    private void OpenLevelUpPanel()
    {
        isLevelUpOpen = true;

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
        }

        RollCards();

        Time.timeScale = 0f;
    }

    private void RollCards()
    {
        List<UpgradeCardData> pool =
            new List<UpgradeCardData>();

        foreach (UpgradeCardData card in availableCards)
        {
            if (card != null)
            {
                pool.Add(card);
            }
        }

        for (int i = 0; i < cardSlots.Count; i++)
        {
            LevelUpCardUI slot =
                cardSlots[i];

            if (slot == null)
                continue;

            if (pool.Count == 0)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            slot.gameObject.SetActive(true);

            int randomIndex =
                Random.Range(
                    0,
                    pool.Count
                );

            UpgradeCardData selected =
                pool[randomIndex];

            pool.RemoveAt(randomIndex);

            slot.Setup(
                selected,
                this
            );
        }
    }

    public void SelectCard(
        UpgradeCardData card)
    {
        if (!isLevelUpOpen)
            return;

        if (upgradeController != null)
        {
            upgradeController.ApplyUpgrade(
                card
            );
        }

        pendingLevelUps--;

        // 레벨업이 여러 번 쌓여있으면
        // 다시 카드 3장 표시
        if (pendingLevelUps > 0)
        {
            RollCards();
            return;
        }

        CloseLevelUpPanel();
    }

    private void CloseLevelUpPanel()
    {
        isLevelUpOpen = false;

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}