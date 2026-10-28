using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("다시 시작할 게임 씬의 이름 (예: InGameScene)")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Tooltip("타이틀 화면 씬의 이름 (예: TitleScene)")]
    [SerializeField] private string titleSceneName = "TitleScene";

    private void Start()
    {
        // 씬 진입 시 시간과 커서를 정상적으로 초기화 (안전 장치)
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// 게임 다시 시작 버튼 클릭 시 호출
    /// </summary>
    public void OnClickRestartGame()
    {
        Debug.Log($"Restarting Game... Loading {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 타이틀 화면으로 돌아가기 버튼 클릭 시 호출
    /// </summary>
    public void OnClickReturnToTitle()
    {
        Debug.Log($"Returning to Title... Loading {titleSceneName}");
        SceneManager.LoadScene(titleSceneName);
    }
}