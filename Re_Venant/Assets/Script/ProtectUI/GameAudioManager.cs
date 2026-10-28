using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip defaultBGM;

    [Header("Default Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultBGMVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float defaultSFXVolume = 1f;

    public const string BGMVolumeKey = "BGMVolume";
    public const string SFXVolumeKey = "SFXVolume";

    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }

    private void Awake()
    {
        // 이미 다른 Scene에서 만들어진 AudioManager가 있다면
        // 새로 만들어진 중복 AudioManager 삭제
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Scene이 변경되어도 유지
        DontDestroyOnLoad(gameObject);

        LoadVolumeSettings();

        SetupAudioSources();

        PlayDefaultBGM();
    }

    // =========================================
    // 초기 설정
    // =========================================

    private void LoadVolumeSettings()
    {
        // 예전에 사용하던 MasterVolume이 있으면
        // 초기값으로 활용
        float oldMasterVolume =
            PlayerPrefs.GetFloat(
                "MasterVolume",
                1f
            );

        BGMVolume =
            PlayerPrefs.GetFloat(
                BGMVolumeKey,
                Mathf.Min(
                    defaultBGMVolume,
                    oldMasterVolume
                )
            );

        SFXVolume =
            PlayerPrefs.GetFloat(
                SFXVolumeKey,
                Mathf.Min(
                    defaultSFXVolume,
                    oldMasterVolume
                )
            );

        BGMVolume =
            Mathf.Clamp01(BGMVolume);

        SFXVolume =
            Mathf.Clamp01(SFXVolume);
    }

    private void SetupAudioSources()
    {
        if (bgmSource != null)
        {
            bgmSource.volume =
                BGMVolume;

            bgmSource.loop = true;

            // BGM은 2D 사운드
            bgmSource.spatialBlend = 0f;
        }

        if (sfxSource != null)
        {
            sfxSource.volume =
                SFXVolume;

            sfxSource.loop = false;

            // 현재 효과음도 2D 방식
            sfxSource.spatialBlend = 0f;
        }
    }

    // =========================================
    // BGM
    // =========================================

    private void PlayDefaultBGM()
    {
        if (bgmSource == null)
            return;

        if (defaultBGM == null)
            return;

        // 이미 음악이 재생되고 있으면
        // 다시 처음부터 재생하지 않음
        if (bgmSource.isPlaying)
            return;

        bgmSource.clip =
            defaultBGM;

        bgmSource.loop = true;

        bgmSource.Play();
    }

    public void PlayBGM(
        AudioClip clip,
        bool restartIfSame = false)
    {
        if (bgmSource == null)
            return;

        if (clip == null)
            return;

        // 같은 BGM이면 다시 재생하지 않음
        if (bgmSource.clip == clip &&
            bgmSource.isPlaying &&
            !restartIfSame)
        {
            return;
        }

        bgmSource.clip =
            clip;

        bgmSource.loop = true;

        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // =========================================
    // SFX
    // =========================================

    public void PlaySFX(
        AudioClip clip,
        float volumeScale = 1f)
    {
        if (sfxSource == null)
            return;

        if (clip == null)
            return;

        sfxSource.PlayOneShot(
            clip,
            Mathf.Clamp01(volumeScale)
        );
    }

    // =========================================
    // Volume
    // =========================================

    public void SetBGMVolume(float value)
    {
        BGMVolume =
            Mathf.Clamp01(value);

        if (bgmSource != null)
        {
            bgmSource.volume =
                BGMVolume;
        }

        PlayerPrefs.SetFloat(
            BGMVolumeKey,
            BGMVolume
        );

        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume =
            Mathf.Clamp01(value);

        if (sfxSource != null)
        {
            sfxSource.volume =
                SFXVolume;
        }

        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            SFXVolume
        );

        PlayerPrefs.Save();
    }
}