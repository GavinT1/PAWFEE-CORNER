using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currency")]
    public int coins = 0;
    public int gems = 0;
    public int xp = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI xpText;

    [Header("Mobile Auto Save Settings")]
    private float saveTimer = 0;
    private const float SaveInterval = 60f;

    private void Awake() 
    { 
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    { 
        UpdateUI();
        LoadGame(); 
    }

    private void Update()
    {
        // auto save timer runs in the background
        saveTimer += Time.deltaTime;
        if(saveTimer >= SaveInterval)
        {
            saveTimer = 0f;
            SaveGame();
            Debug.Log("Optimized Auto-Save Executed.");
        }
    }

//---- COINS------------------------------------------------------------------------
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            SaveGame();
            return true;
        }
        Debug.Log("Not enough coins!");
        return false;

    }

    public bool HasEnoughCoins (int amount)
    {
        return coins >= amount;
    }

//---- GEMS------------------------------------------------------------------------
    public void AddGems(int amount)
    {
        gems += amount;
        UpdateUI();
    }
    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            UpdateUI();
            SaveGame();
            return true;
        }
        Debug.Log("Not enough gems!");
        return false;
    }

    public bool HasEnoughGems (int amount)
    {
        return gems >= amount;
    }

//---- XP ------------------------------------------------------------------------
    public void AddXP(int amount)
    {
        xp += amount;
        UpdateUI();
    }   

//---- REWARDS ------------------------------------------------------------------------
    
    public void AddRewards(int coinAmount, int xpAmount)
    {
        coins += coinAmount;
        xp += xpAmount;
        UpdateUI();
    }

// --- UI ------------------------------------------------------------------------
    void UpdateUI()
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
        if (gemText != null) gemText.text = "Gems: " + gems;
        if (xpText !=  null) xpText.text = "XP: " + xp;
    }


//--- SAVE & LOAD -----------------------------------------------------------------
    public void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame();
        }
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }
    
    public void SaveGame()
    {
        if(SaveSystem.Instance != null)
            SaveSystem.Instance.Save();
    }

    public void LoadGame()
    {
        if(SaveSystem.Instance == null) return;

        GameData data = SaveSystem.Instance.Load();

        coins = data.coins;
        gems = data.gems;
        xp = data.xp;
        
        // load level data and update level UI
        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel = data.currentLevel;
            LevelSystem.Instance.RefreshUI();
        }
        UpdateUI();
        Debug.Log("Loaded — Coins: " + coins + " Gems: " + gems + " XP: " + xp + " Level " + data.currentLevel);     
    }
}    