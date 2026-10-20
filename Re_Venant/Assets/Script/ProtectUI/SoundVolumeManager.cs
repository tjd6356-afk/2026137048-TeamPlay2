using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoundVolumeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Slider soundSlider;

    [Header("View")]
    [SerializeField] private PlayerViewModeManager viewModeManager;

    [Header("Sound Settings")]
    [SerializeField] private float defaultVolume = 1f;

    [Header("Level EXP Settings")]
    [SerializeField] private Slider levelSlider;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private PlayerExperience playerExperience;

    [SerializeField] private float defaultExpMultiplier = 1f;

    private const string ExpMultiplierKey =
        "ExperienceMultiplier";

    private const string VolumeKey = "MasterVolume";

    private bool isSettingsOpen = false;

    public bool IsSettingsOpen => isSettingsOpen;

    private void Start()
    {
        // -----------------------------
        // 설정창 초기화
        // -----------------------------

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        isSettingsOpen = false;

        // -----------------------------
        // 볼륨 초기화
        // -----------------------------

        if (soundSlider != null)
        {
            float savedVolume =
                PlayerPrefs.GetFloat(
                    VolumeKey,
                    defaultVolume
                );

            savedVolume =
                Mathf.Clamp01(savedVolume);

            soundSlider.value =
                savedVolume;

            AudioListener.volume =
                savedVolume;

            soundSlider.onValueChanged.AddListener(
                SetVolume
            );

            // -----------------------------
            // 경험치 배율 초기화
            // -----------------------------

            // -----------------------------
            // 경험치 배율 초기화
            // -----------------------------

            if (playerExperience == null)
            {
                playerExperience =
                    FindFirstObjectByType<PlayerExperience>();
            }

            if (levelSlider != null)
            {
                // EXP 배율 Slider는 무조건 x1 ~ x5
                levelSlider.minValue = 1f;
                levelSlider.maxValue = 5f;
                levelSlider.wholeNumbers = true;

                float savedMultiplier =
                    PlayerPrefs.GetFloat(
                        ExpMultiplierKey,
                        defaultExpMultiplier
                    );

                savedMultiplier =
                    Mathf.Clamp(
                        savedMultiplier,
                        1f,
                        5f
                    );

                // 이벤트 연결 전에 값부터 설정
                levelSlider.SetValueWithoutNotify(
                    savedMultiplier
                );

                ApplyExperienceMultiplier(
                    savedMultiplier
                );

                levelSlider.onValueChanged.AddListener(
                    SetExperienceMultiplier
                );
            }
        }

        // -----------------------------
        // 시점 시스템 자동 검색
        // -----------------------------

        if (viewModeManager == null)
        {
            viewModeManager =
                FindFirstObjectByType<PlayerViewModeManager>();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            // LevelUp / GameOver 등
            // 다른 시스템이 이미 게임을 정지시켰다면
            // 설정창을 새로 열지 않음
            if (!isSettingsOpen &&
                Time.timeScale <= 0f)
            {
                return;
            }

            ToggleSettings();
        }
    }

    private void ToggleSettings()
    {
        if (isSettingsOpen)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    private void OpenSettings()
    {
        isSettingsOpen = true;

        // 설정창 표시
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }

        // 게임 시간 완전 정지
        Time.timeScale = 0f;

        // 3인칭 / 1인칭에서도
        // 마우스를 사용할 수 있도록 설정
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

        Debug.Log("SETTING OPEN - GAME PAUSED");
    }

    private void CloseSettings()
    {
        isSettingsOpen = false;

        // 설정창 닫기
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        // 게임 시간 재개
        Time.timeScale = 1f;

        // 현재 시점에 맞는
        // 마우스 상태로 복구
        if (viewModeManager != null)
        {
            viewModeManager.SetUIMode(false);
        }

        Debug.Log("SETTING CLOSED - GAME RESUMED");
    }

    public void SetVolume(float volume)
    {
        volume =
            Mathf.Clamp01(volume);

        AudioListener.volume =
            volume;

        PlayerPrefs.SetFloat(
            VolumeKey,
            volume
        );

        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveListener(
                SetVolume
            );
        }

        if (levelSlider != null)
        {
            levelSlider.onValueChanged.RemoveListener(
                SetExperienceMultiplier
            );
        }
    }

    public void SetExperienceMultiplier(float value)
    {
        float multiplier =
            Mathf.Clamp(
                Mathf.Round(value),
                1f,
                5f
            );

        ApplyExperienceMultiplier(
            multiplier
        );

        PlayerPrefs.SetFloat(
            ExpMultiplierKey,
            multiplier
        );

        PlayerPrefs.Save();
    }

    private void ApplyExperienceMultiplier(
    float multiplier)
    {
        if (playerExperience != null)
        {
            playerExperience.SetExperienceMultiplier(
                multiplier
            );
        }

        if (levelText != null)
        {
            levelText.text =
                $"EXP x{multiplier:0}";
        }
    }
}