using UnityEngine;
using UnityEngine.UI;

public class TitleHelpManager : MonoBehaviour
{
    [Header("Help UI")]
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // 게임 시작 시 도움말 패널은 닫힌 상태
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }

        if (helpButton != null)
        {
            helpButton.onClick.AddListener(
                OpenHelpPanel
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                CloseHelpPanel
            );
        }
    }

    public void OpenHelpPanel()
    {
        if (helpPanel == null)
            return;

        helpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        if (helpPanel == null)
            return;

        helpPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (helpButton != null)
        {
            helpButton.onClick.RemoveListener(
                OpenHelpPanel
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                CloseHelpPanel
            );
        }
    }
}