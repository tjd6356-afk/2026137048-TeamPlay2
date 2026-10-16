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
            scoreManager =
                FindFirstObjectByType<ScoreManager>();
        }

        if (viewModeManager == null)
        {
            viewModeManager =
                FindFirstObjectByType<PlayerViewModeManager>();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // 최종 점수 계산
        if (scoreManager != null)
        {
            scoreManager.FinishGame();
        }

        // 게임 오버 패널 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // ==============================
        // 마우스를 UI 조작 가능 상태로 변경
        // ==============================
        if (viewModeManager != null)
        {
            viewModeManager.SetUIMode(true);
        }
        else
        {
            // 안전장치
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 게임 정지
        Time.timeScale = 0f;

        Debug.Log("GAME OVER");
    }

    public void ReplayGame()
    {
        // 시간 정상화
        Time.timeScale = 1f;

        // Cursor도 우선 정상화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex
        );
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