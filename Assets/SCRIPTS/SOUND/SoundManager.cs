using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source Channels")]
    public AudioSource musicSource;       
    public AudioSource sfxSource;         

    [Header("Cozy Audio Clip Assets")]
    public AudioClip cafeBgm;
    public AudioClip buttonClickSfx;
    public AudioClip coinDropSfx;
    public AudioClip dailyRewardSfx;
    public AudioClip levelUpSfx;
    public AudioClip panelOpenSfx;
    public AudioClip panelCloseSfx;

    // Direct tracking variables mirroring float slider values
    private float musicVolume = 1.0f;
    private float sfxVolume = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadAudioPreferences();
        PlayMusic(cafeBgm); 
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        // PlayOneShot handles multiple sound overlays without cutting clips out
        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ── PUBLIC TRIGGER INTERACTION METHODS ───────────────────────────────────
    public void PlayButtonClick() => PlaySFX(buttonClickSfx);
    public void PlayCoinDrop()    => PlaySFX(coinDropSfx);
    public void PlayDailyReward() => PlaySFX(dailyRewardSfx);
    public void PlayLevelUp()     => PlaySFX(levelUpSfx);
    public void PlayPanelOpen()   => PlaySFX(panelOpenSfx);
    public void PlayPanelClose()  => PlaySFX(panelCloseSfx);

    // ── LIVE SLIDER SETTERS CALLED BY THE SETTINGS MANAGER ───────────────────
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // ── SYNC PREFERENCES FROM JSON SAVE DATA ─────────────────────────────────
    public void LoadAudioPreferences()
    {
        if (SaveSystem.Instance != null)
        {
            GameData data = SaveSystem.Instance.Load();
            if (data != null)
            {
                musicVolume = data.musicOn ? 1.0f : 0.0f;
                sfxVolume = data.sfxOn ? 1.0f : 0.0f;

                if (musicSource != null) musicSource.volume = musicVolume;
                if (sfxSource != null) sfxSource.volume = sfxVolume;
            }
        }
    }
}
