using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerExperience playerExperience;

    [Header("UI")]
    [SerializeField] private GameObject levelUpPanel;

    private bool isLevelUpOpen = false;

    private void Start()
    {
        if (playerExperience == null)
        {
            playerExperience =
                FindFirstObjectByType<PlayerExperience>();
        }

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        if (playerExperience != null)
        {
            playerExperience.OnLevelUp += OpenLevelUpPanel;
        }
    }

    private void OnDestroy()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp -= OpenLevelUpPanel;
        }
    }

    private void OpenLevelUpPanel()
    {
        if (isLevelUpOpen)
            return;

        isLevelUpOpen = true;

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
        }

        // 게임 일시정지
        Time.timeScale = 0f;

        Debug.Log("LEVEL UP PANEL OPEN");
    }

    // 나중에는 카드 선택 버튼이 이 함수를 호출하게 됨
    public void CompleteLevelUpSelection()
    {
        if (!isLevelUpOpen)
            return;

        isLevelUpOpen = false;

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Debug.Log("LEVEL UP COMPLETE");
    }
}