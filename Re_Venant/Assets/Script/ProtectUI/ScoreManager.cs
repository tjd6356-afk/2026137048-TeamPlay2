using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Survival Score")]
    [SerializeField] private int survivalPointsPerSecond = 10;

    [Header("Clear Settings")]
    [Tooltip("체크 시 설정한 목표 점수 도달 시 클리어 씬으로 이동합니다.")]
    [SerializeField] private bool useClearScore = true;
    [SerializeField] private int targetClearScore = 1000;
    [SerializeField] private string clearSceneName = "ClearScene";

    [Header("Optional In-Game UI")]
    [SerializeField] private TMP_Text currentScoreText;

    [Header("Game Over UI")]
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Score Multiplier")]
    [SerializeField] private float scoreMultiplier = 1f;

    public float ScoreMultiplier => scoreMultiplier;
    private float survivalTime = 0f;
    private float secondTimer = 0f;

    private int survivalScore = 0;
    private int killScore = 0;

    private bool gameEnded = false;

    // GameOverManager에서 클리어 상태인지 확인하기 위해 프로퍼티 추가
    public bool IsGameEnded => gameEnded;

    public int SurvivalScore => survivalScore;
    public int KillScore => killScore;

    public int TotalScore => survivalScore + killScore;
    public float SurvivalTime => survivalTime;

    private void Start()
    {
        UpdateCurrentScoreUI();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        survivalTime += Time.deltaTime;
        secondTimer += Time.deltaTime;

        // 1초마다 생존 점수 지급
        while (secondTimer >= 1f)
        {
            // 방어 코드: 프레임 랙으로 인해 클리어 후에도 루프가 도는 것 방지
            if (gameEnded) break;

            secondTimer -= 1f;

            int gainedScore = Mathf.RoundToInt(survivalPointsPerSecond * scoreMultiplier);
            survivalScore += gainedScore;

            UpdateCurrentScoreUI();
            CheckClearCondition();
        }
    }

    public void AddKillScore(int score)
    {
        if (gameEnded || score <= 0)
            return;

        int finalScore = Mathf.RoundToInt(score * scoreMultiplier);
        killScore += finalScore;

        Debug.Log($"Enemy Kill : Base {score} x {scoreMultiplier} = +{finalScore}");

        UpdateCurrentScoreUI();
        CheckClearCondition();
    }

    private void CheckClearCondition()
    {
        if (!useClearScore || gameEnded)
            return;

        if (TotalScore >= targetClearScore)
        {
            GameClear();
        }
    }

    private void GameClear()
    {
        gameEnded = true;

        // 씬 전환 전 확실한 환경 초기화 (커서 해제 및 시간 정상화)
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log($"GAME CLEAR! Target Score ({targetClearScore}) Reached. Loading {clearSceneName}...");

        SceneManager.LoadScene(clearSceneName);
    }

    public void FinishGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        UpdateFinalScoreUI();
        Debug.Log($"FINAL SCORE : {TotalScore}");
    }

    private void UpdateCurrentScoreUI()
    {
        if (currentScoreText == null) return;
        currentScoreText.text = $"SCORE : {TotalScore}";
    }

    private void UpdateFinalScoreUI()
    {
        if (finalScoreText == null) return;
        finalScoreText.text = $"FINAL SCORE\n{TotalScore}";
    }

    public void IncreaseScoreMultiplier(float amount)
    {
        if (amount <= 0f) return;
        scoreMultiplier += amount;
        Debug.Log($"Score Multiplier : x{scoreMultiplier}");
    }
}