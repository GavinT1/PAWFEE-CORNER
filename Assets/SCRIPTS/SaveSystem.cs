using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

[System.Serializable]
public class GameData
{
    // Currencies
    public int coins;
    public int gems;
    public int xp;

    // Level
    public int currentLevel;

    // Upgrades
    public int tableLevel;
    public int kitchenLevel;
    public int staffLevel;
    public int recipeLevel;

    // Booster
    public bool is2xActive;
    public int boosterCharges;
    public string boosterEndTime;

    // Idle earnings
    public string lastSessionTime;

    // Daily reward
    public string lastDailyRewardClaimed;
    public int dailyRewardStreak;

    // Animal Stars
    public float animalStars;

    // Settings
    public bool musicOn;
    public bool sfxOn;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/pawfeesave.json";
    }

    // ── SAVE ───────────────────────────────────────
    public void Save()
    {
        GameData data = new GameData();

        // Currencies
        data.coins = GameManager.Instance.coins;
        data.gems  = GameManager.Instance.gems;
        data.xp    = GameManager.Instance.xp;

        // Level
        data.currentLevel = LevelSystem.Instance != null
            ? LevelSystem.Instance.currentLevel
            : 1;

        // Animal Stars
        data.animalStars = AnimalStarsManager.Instance != null
            ? AnimalStarsManager.Instance.animalStars: 1.0f;


        // Daily reward 
        data.lastDailyRewardClaimed = DailyRewardManager.Instance != null
            ? DailyRewardManager.Instance.lastClaimedDate: "";
        data.dailyRewardStreak      = DailyRewardManager.Instance != null
            ? DailyRewardManager.Instance.currentStreak: 0;

        // BoosterCharge
        data.boosterCharges = GameManager.Instance.boosterCharges;

        // Last session time
        data.lastSessionTime = System.DateTime.Now.ToString();

        // Settings
        data.musicOn = SettingsManager.Instance != null
            ? SettingsManager.Instance.musicToggle.isOn: true;
        data.sfxOn   = SettingsManager.Instance != null
            ? SettingsManager.Instance.sfxToggle.isOn: true;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);
    }

    // ── LOAD ───────────────────────────────────────
    public GameData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found. Starting fresh.");
            return GetDefaultData();
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        Debug.Log("Game loaded from: " + savePath);
        return data;
    }

    // ── DELETE SAVE ────────────────────────────────
    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
    }

    // ── DEFAULT DATA ───────────────────────────────
    GameData GetDefaultData()
    {
        return new GameData
        {
            coins                  = 0,
            gems                   = 0,
            xp                     = 0,
            currentLevel           = 1,
            tableLevel             = 1,
            kitchenLevel           = 1,
            staffLevel             = 1,
            recipeLevel            = 1,
            is2xActive             = false,
            boosterEndTime         = "",
            lastSessionTime        = System.DateTime.Now.ToString(),
            lastDailyRewardClaimed = "",
            dailyRewardStreak      = 0,
            animalStars            = 1.0f,
            musicOn                = true,
            sfxOn                  = true
        };
    }

}