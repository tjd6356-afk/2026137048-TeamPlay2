using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoundVolumeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingPanel;

    [Header("Sound")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("View")]
    [SerializeField]
    private PlayerViewModeManager viewModeManager;

    [Header("Level EXP Settings")]
    [SerializeField] private Slider levelSlider;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private PlayerExperience playerExperience;

    [SerializeField]
    private float defaultExpMultiplier = 1f;

    private const string ExpMultiplierKey =
        "ExperienceMultiplier";

    private bool isSettingsOpen = false;

    public bool IsSettingsOpen =>
        isSettingsOpen;

    private void Start()
    {
        // =====================================
        // Panel
        // =====================================

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        isSettingsOpen = false;

        // =====================================
        // BGM / SFX
        // =====================================

        InitializeSoundSliders();

        // =====================================
        // EXP
        // =====================================

        InitializeExperienceSlider();

        // =====================================
        // View
        // =====================================

        if (viewModeManager == null)
        {
            viewModeManager =
                FindFirstObjectByType<PlayerViewModeManager>();
        }
    }

    // =========================================
    // SOUND INITIALIZE
    // =========================================

    private void InitializeSoundSliders()
    {
        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.wholeNumbers = false;

            float volume =
                GameAudioManager.Instance != null
                ? GameAudioManager.Instance.BGMVolume
                : PlayerPrefs.GetFloat(
                    GameAudioManager.BGMVolumeKey,
                    1f
                );

            bgmSlider.SetValueWithoutNotify(
                volume
            );

            bgmSlider.onValueChanged.AddListener(
                SetBGMVolume
            );
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;

            float volume =
                GameAudioManager.Instance != null
                ? GameAudioManager.Instance.SFXVolume
                : PlayerPrefs.GetFloat(
                    GameAudioManager.SFXVolumeKey,
                    1f
                );

            sfxSlider.SetValueWithoutNotify(
                volume
            );

            sfxSlider.onValueChanged.AddListener(
                SetSFXVolume
            );
        }
    }

    // =========================================
    // EXP INITIALIZE
    // =========================================

    private void InitializeExperienceSlider()
    {
        if (playerExperience == null)
        {
            playerExperience =
                FindFirstObjectByType<PlayerExperience>();
        }

        if (levelSlider == null)
            return;

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

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            // LevelUp 또는 GameOver 때문에 이미
            // 정지된 경우 설정창을 열지 않음
            if (!isSettingsOpen &&
                Time.timeScale <= 0f)
            {
                return;
            }

            ToggleSettings();
        }
    }

    // =========================================
    // SETTINGS PANEL
    // =========================================

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

        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }

        Time.timeScale = 0f;

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

        Debug.Log(
            "SETTING OPEN - GAME PAUSED"
        );
    }

    private void CloseSettings()
    {
        isSettingsOpen = false;

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (viewModeManager != null)
        {
            viewModeManager.SetUIMode(false);
        }

        Debug.Log(
            "SETTING CLOSED - GAME RESUMED"
        );
    }

    // =========================================
    // BGM
    // =========================================

    public void SetBGMVolume(float value)
    {
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.SetBGMVolume(
                value
            );
        }
    }

    // =========================================
    // SFX
    // =========================================

    public void SetSFXVolume(float value)
    {
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.SetSFXVolume(
                value
            );
        }
    }

    // =========================================
    // EXPERIENCE
    // =========================================

    public void SetExperienceMultiplier(
        float value)
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

    private void OnDestroy()
    {
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(
                SetBGMVolume
            );
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(
                SetSFXVolume
            );
        }

        if (levelSlider != null)
        {
            levelSlider.onValueChanged.RemoveListener(
                SetExperienceMultiplier
            );
        }
    }
}