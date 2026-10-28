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
    [SerializeField] private Slider soundSlider;

    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 1f;

    // GameScene의 SoundVolumeManager와
    // 반드시 같은 Key를 사용해야 함
    private const string VolumeKey = "MasterVolume";

    private void Start()
    {
        // -----------------------------
        // 설정창은 처음에 닫기
        // -----------------------------

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        // -----------------------------
        // 저장된 볼륨 불러오기
        // -----------------------------

        if (soundSlider != null)
        {
            // Slider 범위
            soundSlider.minValue = 0f;
            soundSlider.maxValue = 1f;
            soundSlider.wholeNumbers = false;

            float savedVolume =
                PlayerPrefs.GetFloat(
                    VolumeKey,
                    defaultVolume
                );

            savedVolume =
                Mathf.Clamp01(savedVolume);

            // 이벤트 호출 없이 Slider 위치 설정
            soundSlider.SetValueWithoutNotify(
                savedVolume
            );

            // 실제 전체 게임 사운드에도 적용
            AudioListener.volume =
                savedVolume;

            soundSlider.onValueChanged.AddListener(
                SetVolume
            );
        }

        // -----------------------------
        // 버튼 연결
        // -----------------------------

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

    // =========================================
    // 설정창 열기
    // =========================================

    public void OpenSettings()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    // =========================================
    // 설정창 닫기
    // =========================================

    public void CloseSettings()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    // =========================================
    // 전체 사운드 변경
    // =========================================

    public void SetVolume(float volume)
    {
        volume =
            Mathf.Clamp01(volume);

        // 전체 사운드 적용
        AudioListener.volume =
            volume;

        // 다른 Scene에서도 사용하도록 저장
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