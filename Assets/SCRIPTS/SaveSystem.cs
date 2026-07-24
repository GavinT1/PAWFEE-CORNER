using UnityEngine;
using System.IO;
using System.Globalization;

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

    // recipe
    public bool[] recipeUnlocks;

    // tables
    public bool[] tableUnlockedStates;
    public int [] tableTiers;
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

        savePath = Path.Combine(Application.persistentDataPath, "pawfeesave.json");
    }

    // ── SAVE ───────────────────────────────────────
    public void Save()
    {
        GameData data = new GameData();

        // Currencies
        if (GameManager.Instance != null)
        {
            data.coins = GameManager.Instance.coins;
            data.gems  = GameManager.Instance.gems;
            data.xp    = GameManager.Instance.xp;
            data.boosterCharges = GameManager.Instance.boosterCharges;
        }

        // Level
        data.currentLevel = LevelSystem.Instance != null
            ? LevelSystem.Instance.currentLevel
            : 1;

        // Tables
        if (UpgradeManager.Instance != null)
        {
            data.tableUnlockedStates = UpgradeManager.Instance.GetTableUnlockStates();
            data.tableTiers = UpgradeManager.Instance.GetTableTiers();
        }

        // Animal Stars
        data.animalStars = AnimalStarsManager.Instance != null
            ? AnimalStarsManager.Instance.animalStars : 1.0f;

        // Daily reward 
        data.lastDailyRewardClaimed = DailyRewardManager.Instance != null
            ? DailyRewardManager.Instance.lastClaimedDate : "";
        data.dailyRewardStreak      = DailyRewardManager.Instance != null
            ? DailyRewardManager.Instance.currentStreak : 0;

        // Last session time (FIXED: InvariantCulture for universal ISO format)
        data.lastSessionTime = System.DateTime.Now.ToString("o", CultureInfo.InvariantCulture);

        // Settings
        if (SettingsManager.Instance != null && SettingsManager.Instance.musicSlider != null)
        {
            data.musicOn = SettingsManager.Instance.musicSlider.value > 0.01f;
        }
        else
        {
            data.musicOn = PlayerPrefs.GetFloat("MusicVolume", 1.0f) > 0.01f;
        }

        if (SettingsManager.Instance != null && SettingsManager.Instance.sfxSlider != null)
        {
            data.sfxOn = SettingsManager.Instance.sfxSlider.value > 0.01f;
        }
        else
        {
            data.sfxOn = PlayerPrefs.GetFloat("SFXVolume", 1.0f) > 0.01f;
        }

        // Saving recipes
        if (RecipeManager.Instance != null)
        {
            data.recipeUnlocks = RecipeManager.Instance.GetRecipeUnlockStates();
        }
        else
        {
            data.recipeUnlocks = new bool[]{ true, false, false, false, false };
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);
    }

    // ── LOAD ───────────────────────────────────────
    public GameData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found. Creating default data.");
            GameData defaultData = GetDefaultData();
            return defaultData;
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
    public GameData GetDefaultData()
    {
        return new GameData
        {
            coins                  = 0,
            gems                   = 0,
            xp                     = 0,
            currentLevel           = 1,
            kitchenLevel           = 1,
            staffLevel             = 1,
            recipeLevel            = 1,
            is2xActive             = false,
            boosterEndTime         = "",
            lastSessionTime        = System.DateTime.Now.ToString("o", CultureInfo.InvariantCulture),
            lastDailyRewardClaimed = "",
            dailyRewardStreak      = 0,
            animalStars            = 1.0f,
            musicOn                = true,
            sfxOn                  = true,
            recipeUnlocks          = new bool[] { true, false, false, false, false },
            tableUnlockedStates    = new bool[] { true, false, false, false, false },
            tableTiers             = new int[] { 1, 1, 1, 1, 1 },  
        };
    }
}