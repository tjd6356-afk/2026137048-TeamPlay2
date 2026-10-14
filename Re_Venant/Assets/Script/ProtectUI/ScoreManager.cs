using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Survival Score")]
    [SerializeField] private int survivalPointsPerSecond = 10;

    [Header("Optional In-Game UI")]
    [SerializeField] private TMP_Text currentScoreText;

    [Header("Game Over UI")]
    [SerializeField] private TMP_Text finalScoreText;

    private float survivalTime = 0f;
    private float secondTimer = 0f;

    private int survivalScore = 0;
    private int killScore = 0;

    private bool gameEnded = false;

    public int SurvivalScore => survivalScore;
    public int KillScore => killScore;

    public int TotalScore =>
        survivalScore + killScore;

    public float SurvivalTime =>
        survivalTime;

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
            secondTimer -= 1f;

            survivalScore +=
                survivalPointsPerSecond;

            UpdateCurrentScoreUI();
        }
    }

    // 적이 죽었을 때 호출
    public void AddKillScore(int score)
    {
        if (gameEnded)
            return;

        if (score <= 0)
            return;

        killScore += score;

        Debug.Log(
            $"Enemy Kill +{score} / Total Score : {TotalScore}"
        );

        UpdateCurrentScoreUI();
    }

    // 게임 오버 순간 호출
    public void FinishGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        UpdateFinalScoreUI();

        Debug.Log(
            $"FINAL SCORE : {TotalScore}"
        );
    }

    private void UpdateCurrentScoreUI()
    {
        if (currentScoreText == null)
            return;

        currentScoreText.text =
            $"SCORE : {TotalScore}";
    }

    private void UpdateFinalScoreUI()
    {
        if (finalScoreText == null)
            return;

        int minutes =
            Mathf.FloorToInt(survivalTime / 60f);

        int seconds =
            Mathf.FloorToInt(survivalTime % 60f);

        finalScoreText.text =
            $"SURVIVAL  {minutes:00}:{seconds:00}\n" +
            $"SURVIVAL SCORE  {survivalScore}\n" +
            $"KILL SCORE  {killScore}\n\n" +
            $"FINAL SCORE  {TotalScore}";
    }
}