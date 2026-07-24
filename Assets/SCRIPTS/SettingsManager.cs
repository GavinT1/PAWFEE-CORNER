using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Slider References")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Sources (Scene Fallbacks)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Allowed to overwrite or re-bind securely across scene shifts
        Instance = this;
    }

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        LoadSettings();
    }

    //--- OPEN/CLOSE-------------------------
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelOpen();
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelClose();
    }

    //--- SLIDER LOGIC DRIVERS ────────────────────────────────────────────────
    public void OnMusicSliderChanged()
    {
        if (musicSlider == null) return;
        float value = musicSlider.value; // Float between 0.0 and 1.0

        // Update local scene fallback channel if it exists
        if (musicSource != null) musicSource.volume = value;

        // Push volume scaling dynamically down into the global persistent SoundManager channels
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(value);
        }

        SaveSettings();
        Debug.Log("Live Music Volume Set To: " + value);
    }

    public void OnSFXSliderChanged()
    {
        if (sfxSlider == null) return;
        float value = sfxSlider.value; // Float between 0.0 and 1.0

        // Update local scene fallback channel if it exists
        if (sfxSource != null) sfxSource.volume = value;

        // Push volume scaling dynamically down into the global persistent SoundManager channels
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXVolume(value);
        }

        SaveSettings();
        Debug.Log("Live SFX Volume Set To: " + value);
    }

    //--- SAVE AND LOAD ------------------------------
    void SaveSettings()
    {
        if (SaveSystem.Instance != null)
        {
            // Convert values over smoothly to save variables
            GameData data = SaveSystem.Instance.Load();
            if (data != null)
            {
                // If slider is completely turned down to 0, save as muted/false
                data.musicOn = (musicSlider != null) ? (musicSlider.value > 0.01f) : true;
                data.sfxOn = (sfxSlider != null) ? (sfxSlider.value > 0.01f) : true;
            }
            SaveSystem.Instance.Save();
        }
    }

    void LoadSettings()
    {
        if (SaveSystem.Instance == null) return;

        GameData data = SaveSystem.Instance.Load();
        
        // Re-align your UI element filling tracking metrics
        if (musicSlider != null)
        {
            musicSlider.value = data.musicOn ? 1.0f : 0.0f;
            if (musicSource != null) musicSource.volume = musicSlider.value;
            if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = data.sfxOn ? 1.0f : 0.0f;
            if (sfxSource != null) sfxSource.volume = sfxSlider.value;
            if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(sfxSlider.value);
        }
    }
}
