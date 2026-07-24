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

        // Tables
        if (UpgradeManager.Instance != null)
        {
            data.tableUnlockedStates = UpgradeManager.Instance.GetTableUnlockStates();
            data.tableTiers = UpgradeManager.Instance.GetTableTiers();
        }

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
        if (SettingsManager.Instance != null && SettingsManager.Instance.musicSlider != null)
        {
            // If the slider handle is dragged above 0, music is saved as active (true)
            data.musicOn = SettingsManager.Instance.musicSlider.value > 0.01f;
        }
        else
        {
            data.musicOn = true;
        }

        if (SettingsManager.Instance != null && SettingsManager.Instance.sfxSlider != null)
        {
            data.sfxOn = SettingsManager.Instance.sfxSlider.value > 0.01f;
        }
        else
        {
            data.sfxOn = true;
        }


        // saving recipes
        if (RecipeManager.Instance != null)
        {
            data.recipeUnlocks = RecipeManager.Instance.GetRecipeUnlockStates();
        }
        else
        {
            data.recipeUnlocks = new bool[]{ true, false, false, false, false};
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

    // ── COMPLETE DATA RESET ────────────────────────
    public void ResetAllGameData()
    {
        // 1. Delete JSON File
        DeleteSave();

        // 2. Wipe PlayerPrefs (Daily Reward Keys)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("ALL SAVE DATA AND PLAYERPREFS WIPED CLEAN!");
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
            sfxOn                  = true,
            recipeUnlocks = new bool[] { true, false, false, false, false},
            tableUnlockedStates    = new bool[] { true, false, false, false, false }, // First table is unlocked
            tableTiers             = new int[] { 1, 1, 1, 1, 1 },  
        };
    }
}