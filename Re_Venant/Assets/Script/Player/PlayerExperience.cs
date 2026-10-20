using System;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;

    [Header("Experience")]
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int baseRequiredExp = 10;

    [Header("Experience Multiplier")]
    [SerializeField] private float experienceMultiplier = 1f;

    public float ExperienceMultiplier => experienceMultiplier;

    [Header("Level Scaling")]
    [SerializeField] private float requiredExpMultiplier = 1.5f;

    private int requiredExp;

    // UI용 이벤트
    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelChanged;

    // LevelUpManager가 받을 이벤트
    public event Action OnLevelUp;

    public int CurrentLevel => currentLevel;
    public int CurrentExp => currentExp;
    public int RequiredExp => requiredExp;

    private void Awake()
    {
        CalculateRequiredExp();
    }

    private void Start()
    {
        NotifyUI();
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
            return;

        // 설정된 경험치 배율 적용
        int finalAmount =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    amount * experienceMultiplier
                )
            );

        currentExp += finalAmount;

        Debug.Log(
            $"EXP +{finalAmount} " +
            $"(Base {amount} x {experienceMultiplier}) / " +
            $"{currentExp} / {requiredExp}"
        );

        CheckLevelUp();

        NotifyUI();
    }

    public void SetExperienceMultiplier(float multiplier)
    {
        experienceMultiplier =
            Mathf.Max(1f, multiplier);

        Debug.Log(
            $"EXP 배율 변경 : x{experienceMultiplier}"
        );
    }

    private void CheckLevelUp()
    {
        // 큰 경험치를 한번에 먹었을 경우를 대비
        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;

            currentLevel++;

            CalculateRequiredExp();

            Debug.Log(
                $"LEVEL UP! 현재 레벨 : {currentLevel}"
            );

            OnLevelChanged?.Invoke(currentLevel);
            OnLevelUp?.Invoke();
        }
    }

    private void CalculateRequiredExp()
    {
        requiredExp = Mathf.CeilToInt(
            baseRequiredExp *
            Mathf.Pow(
                requiredExpMultiplier,
                currentLevel - 1
            )
        );
    }

    private void NotifyUI()
    {
        OnExpChanged?.Invoke(
            currentExp,
            requiredExp
        );

        OnLevelChanged?.Invoke(
            currentLevel
        );
    }
}