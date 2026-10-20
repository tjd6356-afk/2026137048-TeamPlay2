using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private PlayerExperience playerExperience;

    [SerializeField]
    private PlayerUpgradeController upgradeController;

    [SerializeField]
    private PlayerBulletHealth bulletHealth;

    [Header("UI")]
    [SerializeField]
    private GameObject levelUpPanel;

    [Header("Cards")]
    [SerializeField]
    private List<UpgradeCardData> availableCards =
        new List<UpgradeCardData>();

    [SerializeField]
    private List<LevelUpCardUI> cardSlots =
        new List<LevelUpCardUI>();

    [Header("Reroll")]
    [SerializeField]
    private Button rerollButton;

    [SerializeField]
    private TMP_Text rerollText;

    [SerializeField]
    private int freeRerollsPerLevel = 1;

    [SerializeField]
    private int rerollBulletCost = 10;

    private int freeRerollsRemaining;

    private bool isLevelUpOpen = false;

    // 경험치를 많이 먹어서
    // 레벨업이 여러 번 예약되었을 경우
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

        if (bulletHealth == null)
        {
            bulletHealth =
                FindFirstObjectByType<PlayerBulletHealth>();
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

        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(
                RerollCards
            );
        }
    }

    private void OnDestroy()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp -=
                OnPlayerLevelUp;
        }

        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveListener(
                RerollCards
            );
        }
    }

    // =========================================
    // LEVEL UP
    // =========================================

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

        // 새로운 레벨업이므로
        // 무료 리롤 초기화
        ResetRerolls();

        RollCards();

        Time.timeScale = 0f;
    }

    // =========================================
    // CARD RANDOM
    // =========================================

    private void RollCards()
    {
        List<UpgradeCardData> pool =
            new List<UpgradeCardData>();

        foreach (UpgradeCardData card
                 in availableCards)
        {
            if (card != null)
            {
                pool.Add(card);
            }
        }

        for (int i = 0;
             i < cardSlots.Count;
             i++)
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

            UpgradeCardData selectedCard =
                pool[randomIndex];

            // 같은 카드가 같은 3장 안에서
            // 중복 등장하지 않도록 제거
            pool.RemoveAt(randomIndex);

            slot.Setup(
                selectedCard,
                this
            );
        }
    }

    // =========================================
    // REROLL
    // =========================================

    private void ResetRerolls()
    {
        freeRerollsRemaining =
            freeRerollsPerLevel;

        UpdateRerollUI();
    }

    private void RerollCards()
    {
        if (!isLevelUpOpen)
            return;

        if (bulletHealth == null)
            return;

        if (bulletHealth.IsDead)
            return;

        // -------------------------------------
        // 무료 리롤
        // -------------------------------------

        if (freeRerollsRemaining > 0)
        {
            freeRerollsRemaining--;

            Debug.Log("무료 리롤 사용");

            RollCards();

            UpdateRerollUI();

            return;
        }

        // -------------------------------------
        // 유료 리롤
        // -------------------------------------

        Debug.Log(
            $"유료 리롤 : 탄환 {rerollBulletCost} 소모"
        );

        bool survived =
            bulletHealth.SpendBulletsForReroll(
                rerollBulletCost
            );

        // 리롤 비용으로 Player가 사망
        if (!survived)
        {
            HandleRerollDeath();

            return;
        }

        // 살아있다면 카드 다시 생성
        RollCards();

        UpdateRerollUI();
    }

    private void UpdateRerollUI()
    {
        if (rerollText == null)
            return;

        if (freeRerollsRemaining > 0)
        {
            rerollText.text =
                $"무료 리롤 ({freeRerollsRemaining})";
        }
        else
        {
            int currentBullets = 0;

            if (bulletHealth != null)
            {
                currentBullets =
                    bulletHealth.CurrentBullets;
            }

            rerollText.text =
                $"리롤  -{rerollBulletCost} 탄환\n" +
                $"현재 {currentBullets}";
        }
    }

    private void HandleRerollDeath()
    {
        // PlayerBulletHealth.Die()에서
        // 이미 GameOverManager.GameOver()가 실행됨.

        // LevelUpPanel이 GameOverPanel과
        // 겹쳐 보이지 않도록 숨김
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        isLevelUpOpen = false;

        // 중요:
        // 여기서는 Time.timeScale = 1을 하면 안 됨.
        // GameOver 상태이므로 계속 정지 상태여야 함.

        Debug.Log(
            "리롤 비용으로 Player가 사망했습니다."
        );
    }

    // =========================================
    // CARD SELECT
    // =========================================

    public void SelectCard(
        UpgradeCardData card)
    {
        if (!isLevelUpOpen)
            return;

        if (card == null)
            return;

        if (upgradeController != null)
        {
            upgradeController.ApplyUpgrade(
                card
            );
        }

        pendingLevelUps--;

        // 경험치를 한번에 많이 먹어서
        // 또 레벨업해야 하는 경우
        if (pendingLevelUps > 0)
        {
            // 다음 Level Up이므로
            // 무료 리롤도 다시 지급
            ResetRerolls();

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