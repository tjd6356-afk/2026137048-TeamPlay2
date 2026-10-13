using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver = false;

    private void Awake()
    {
        // 게임 시작 시 패널 숨기기
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 혹시 이전 Scene에서 멈춰 있었다면 정상화
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // 게임 오버 패널 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 게임 정지
        Time.timeScale = 0f;

        Debug.Log("GAME OVER");
    }

    public void ReplayGame()
    {
        // 반드시 시간 정상화 후 Scene 재로드
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ExitGame()
    {
        // 실제 빌드된 게임 종료
        Application.Quit();

#if UNITY_EDITOR
        // Unity Editor에서 Play Mode 종료
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}