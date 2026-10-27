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

        // 최종 점수 확정
        if (scoreManager != null)
        {
            scoreManager.FinishGame();
        }

        // GameOverPanel 활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // UI 레이아웃 위치 즉시 갱신
        Canvas.ForceUpdateCanvases();

        // GameOverUIAnimator가 Inspector에 연결되지 않았으면
        // GameOverPanel 내부에서 자동 검색
        if (gameOverUIAnimator == null &&
            gameOverPanel != null)
        {
            gameOverUIAnimator =
                gameOverPanel.GetComponentInChildren<GameOverUIAnimator>(true);
        }

        // 마우스 활성화
        if (viewModeManager != null)
        {
            viewModeManager.SetUIMode(true);
        }
        else
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }

        // 게임 정지
        Time.timeScale = 0f;

        // GameOver 연출
        if (gameOverUIAnimator != null)
        {
            Debug.Log("GameOver Animation START");

            gameOverUIAnimator.PlayGameOverAnimation();
        }
        else
        {
            Debug.LogError(
                "GameOverUIAnimator를 찾지 못했습니다!"
            );
        }

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