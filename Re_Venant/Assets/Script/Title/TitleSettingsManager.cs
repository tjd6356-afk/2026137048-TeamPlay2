using UnityEngine;
using UnityEngine.UI;

public class TitleSettingsManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingPanel;

    [Header("Buttons")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Button closeButton;

    [Header("Sound")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 설정창 처음에는 닫기
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        InitializeSoundUI();

        // 버튼
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(
                OpenSettings
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                CloseSettings
            );
        }
    }

    private void InitializeSoundUI()
    {
        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.wholeNumbers = false;

            float volume = 1f;

            if (GameAudioManager.Instance != null)
            {
                volume =
                    GameAudioManager.Instance.BGMVolume;
            }
            else
            {
                volume =
                    PlayerPrefs.GetFloat(
                        GameAudioManager.BGMVolumeKey,
                        1f
                    );
            }

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

            float volume = 1f;

            if (GameAudioManager.Instance != null)
            {
                volume =
                    GameAudioManager.Instance.SFXVolume;
            }
            else
            {
                volume =
                    PlayerPrefs.GetFloat(
                        GameAudioManager.SFXVolumeKey,
                        1f
                    );
            }

            sfxSlider.SetValueWithoutNotify(
                volume
            );

            sfxSlider.onValueChanged.AddListener(
                SetSFXVolume
            );
        }
    }

    // =========================================
    // Panel
    // =========================================

    public void OpenSettings()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
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

        if (settingButton != null)
        {
            settingButton.onClick.RemoveListener(
                OpenSettings
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                CloseSettings
            );
        }
    }
}