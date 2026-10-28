using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Score")]
    [SerializeField] private ScoreManager scoreManager;

    [Header("View")]
    [SerializeField] private PlayerViewModeManager viewModeManager;

    [Header("Game Over Animation")]
    [SerializeField]
    private GameOverUIAnimator gameOverUIAnimator;

    private bool isGameOver = false;

    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        if (viewModeManager == null)
        {
            viewModeManager = FindFirstObjectByType<PlayerViewModeManager>();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        // ★ 추가된 핵심 방어 코드 ★
        // 스코어 매니저가 이미 게임 종료(클리어 등)를 선언했다면 게임 오버 처리를 무시함
        if (scoreManager != null && scoreManager.IsGameEnded)
            return;

        isGameOver = true;

        // 이하 기존 코드 동일하게 유지
        if (scoreManager != null)
        {
            scoreManager.FinishGame();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();

        if (gameOverUIAnimator == null && gameOverPanel != null)
        {
            gameOverUIAnimator = gameOverPanel.GetComponentInChildren<GameOverUIAnimator>(true);
        }

        if (viewModeManager != null)
        {
            viewModeManager.SetUIMode(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Time.timeScale = 0f;

        if (gameOverUIAnimator != null)
        {
            Debug.Log("GameOver Animation START");
            gameOverUIAnimator.PlayGameOverAnimation();
        }
        else
        {
            Debug.LogError("GameOverUIAnimator를 찾지 못했습니다!");
        }

        Debug.Log("GAME OVER");
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}