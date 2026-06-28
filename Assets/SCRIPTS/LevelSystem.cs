using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance;

    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public UnityEngine.UI.Slider xpSlider;

    [Header("Level Settings")]
    public int currentLevel = 1 ;
    public int maxLevel = 10;

    private int[] xpThresholds = new int[]
    {
        0,     // Level 1 
        500,   // Level 2
        1500,  // Level 3
        3500,  // Level 4
        7000,  // Level 5
        13000, // Level 6
        23000, // Level 7
        39000, // Level 8
        64000, // Level 9
        102000 // Level 10
    };

    private int[] xpToNextLevel = new int[]
    {
        500,   // Level 1 to 2
        1000,  // Level 2 to 3
        2000,  // Level 3 to 4
        3500,  // Level 4 to 5 
        6000,  // Level 5 to 6
        10000, // Level 6 to 7
        16000, // Level 7 to 8
        25000, // Level 8 to 9
        38000  // level 9 to 10
    };

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        CheckLevelUp();
    }

    // --- CALLED BY GAME MANAGER AFTER XP ADDED------------------------------------
    public void CheckLevelUp()
    {
        if (currentLevel >= maxLevel) return;
        
        int currentXP = 0;
        if (GameManager.Instance != null)
        {
            currentXP = GameManager.Instance.xp;
        }

        while (currentLevel < maxLevel && currentXP >= xpThresholds[currentLevel])
        {
            currentLevel++;
            OnLevelUp(currentLevel);
        }

        UpdateLevelUI();

        // refresh upgrade buttons when level change
        if (UpgradeManager.Instance != null) UpgradeManager.Instance.OnPlayerLevelUp();

        // Automatically updates card colors/unlocks the moment the level changes
        if (RecipeManager.Instance != null) RecipeManager.Instance.RefreshRecipeUI();
    
    }

    //--- LEVEL UP EVENT -----------------------------------------------
    void OnLevelUp(int newLevel)
    {
        Debug.Log("LEVEL UP! Now Level " + newLevel);

        // tier unloock at odd levels
        if (newLevel % 2 != 0)
        {
            int newTier = (newLevel /2) + 1;
            Debug.Log("Tier " + newTier + " unlocked for all upgrade categories!");
        }

        //Staff unlocks
        switch (newLevel)
        {
            case 3: 
            Debug.Log("Fox Chef unloacked!");    
            break;
            case 5: 
            Debug.Log("Cat Cashier unlocked!");
            break;
            case 7:
            Debug.Log("Bear Baker unlocked! ");
            break;
            case 8:
            case 10: 
            Debug.Log("Penguin Waiter unlocked!");
            break;
        }

        if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
    }

    //--- XP PROGRESS--------------------------------------------------
    public float GetXPBarProgress()
    {
        if(currentLevel >= maxLevel) return 1f;

        int currentXP = 0;
        if (GameManager.Instance != null)
        {
            currentXP = GameManager.Instance.xp;
        }

        int levelStartXP = xpThresholds[currentLevel -1];
        int levelXPNeeded = xpToNextLevel[currentLevel -1];
        int xpIntoLevel = currentXP - levelStartXP;

        // Return raw progress matching our slider scale configuration perfectly
        return Mathf.Clamp(xpIntoLevel, 0, levelXPNeeded);
    }

    //---UI----------------------------------------------------------
    public void UpdateLevelUI()
    {
        if (levelText != null) levelText.text = "level " + currentLevel;
        if(xpSlider != null)
        {
            if (currentLevel >= maxLevel)
            {
                xpSlider.minValue = 0f;
                xpSlider.maxValue = 1f;
                xpSlider.value = 1f;
                return;
            }

            int levelXPNeeded = xpToNextLevel[currentLevel - 1];

            xpSlider.minValue = 0f;
            xpSlider.maxValue = levelXPNeeded;
            xpSlider.value = GetXPBarProgress();
        }
    }

    public void RefreshUI()
    {
        UpdateLevelUI();
    }
}