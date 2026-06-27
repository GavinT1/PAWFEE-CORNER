using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Toggle References")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
    }
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    //--- TOGGLES -------------------------------
    public void OnMusicToggleChanged()
    {
        bool isOn = musicToggle.isOn;

        if(musicSource != null) musicSource.mute = !isOn;

        SaveSettings();
        Debug.Log("Music: " + (isOn ? "ON" : "OFF"));
    }

    public void OnSFXToggleChanged()
    {
        bool isOn = sfxToggle.isOn;

        if (sfxSource != null) sfxSource.mute = !isOn;

        SaveSettings();
        Debug.Log("SFX: " + (isOn ? "ON" : "OFF"));
    }

    //--- SAVE AND LOAD ------------------------------
    void SaveSettings()
    {
        if(SaveSystem.Instance != null) SaveSystem.Instance.Save();
    }

    void LoadSettings()
    {
        if(SaveSystem.Instance == null) return;

        GameData data = SaveSystem.Instance.Load();
        if (musicToggle != null)
        {
            musicToggle.isOn = data.musicOn;
            if (musicSource != null) musicSource.mute = !data.musicOn;
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = data.sfxOn;
            if(sfxSource != null) sfxSource.mute = !data.sfxOn;
        }
    }
}
