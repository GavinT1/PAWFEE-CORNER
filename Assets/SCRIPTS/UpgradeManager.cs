using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Tables Reference")]
    public CafeTable[] tables;

    [Header("UI Button Text Display")]
    public TMP_Text[] buttonTexts;

    // GDD costs per tier
    private int[] tierCosts = new int[]
    {
        150,     // Tier 1
        1200,    // Tier 2
        9000,    // Tier 3
        70000,   // Tier 4
        500000   // Tier 5
    };

    // Level required per tier — matches GDD
    private int[] tierLevelRequirements = new int[]
    {
        1,  // Tier 1 — Level 1
        3,  // Tier 2 — Level 3
        5,  // Tier 3 — Level 5
        7,  // Tier 4 — Level 7
        9   // Tier 5 — Level 9
    };

    // Unlock costs per table slot
    private int[] unlockCosts = new int[]
    {
        0,    // Table 1 — free from start
        150,  // Table 2
        300,  // Table 3
        500,  // Table 4
        800   // Table 5
    };

    public static UpgradeManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    void Start()
    {   
        if (SaveSystem.Instance != null)
        {
            GameData data = SaveSystem.Instance.Load();
            
            // Check if a saved file array exists and matches our scene layout array size
            if (data != null && data.tableUnlockedStates != null && data.tableUnlockedStates.Length == tables.Length)
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    if (tables[i] != null)
                    {
                        tables[i].isUnlocked = data.tableUnlockedStates[i];
                        tables[i].tableLevel = data.tableTiers[i];
                        
                        // Physically toggle the table gameobject active/inactive in your scene space
                        tables[i].gameObject.SetActive(data.tableUnlockedStates[i]);
                    }
                }
            }
        }
        
        UpdateUpgradeUI();
    }

    public void OnClickUpgradeRow(int index)
    {
        if (index < 0 || index >= tables.Length) return;

        CafeTable targetTable = tables[index];
        int playerLevel = LevelSystem.Instance != null
            ? LevelSystem.Instance.currentLevel
            : 1;

        // ── UNLOCK TABLE ───────────────────────────
        if (!targetTable.isUnlocked)
        {
            if (GameManager.Instance.SpendCoins(unlockCosts[index]))
            {
                targetTable.isUnlocked = true;
                targetTable.gameObject.SetActive(true);
                GameManager.Instance.AddXP(25);
                UpdateUpgradeUI();
                if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
            }
            else
            {
                Debug.Log("Not enough coins to unlock Table " + (index + 1));
            }
            return;
        }

        // ── UPGRADE TABLE ──────────────────────────
        int currentTier = targetTable.tableLevel;

        // Already max tier
        if (currentTier >= tierCosts.Length)
        {
            Debug.Log("Table " + (index + 1) + " is already max tier!");
            return;
        }

        // Level gate check
        int requiredLevel = tierLevelRequirements[currentTier];
        if (playerLevel < requiredLevel)
        {
            Debug.Log("Need Level " + requiredLevel
                + " to upgrade to Tier " + (currentTier + 1)
                + ". You are Level " + playerLevel);
            UpdateUpgradeUI();
            return;
        }

        // Spend coins and upgrade
        int cost = tierCosts[currentTier];
        if (GameManager.Instance.SpendCoins(cost))
        {
            targetTable.tableLevel++;
            GameManager.Instance.AddXP(25);
            UpdateUpgradeUI();

            if (SaveSystem.Instance != null) SaveSystem.Instance.Save();

            Debug.Log("Table " + (index + 1)
                + " upgraded to Tier " + targetTable.tableLevel);
        }
        else
        {
            Debug.Log("Not enough coins! Need " + cost);
        }
    }

    void UpdateUpgradeUI()
    {
        int playerLevel = LevelSystem.Instance != null
            ? LevelSystem.Instance.currentLevel
            : 1;

        for (int i = 0; i < tables.Length; i++)
        {
            if (i >= buttonTexts.Length) break;

            CafeTable table = tables[i];

            // Table not yet unlocked
            if (!table.isUnlocked)
            {
                buttonTexts[i].text = "Unlock Table "
                    + (i + 1) + ": "
                    + unlockCosts[i] + " Coins";
                continue;
            }

            int currentTier = table.tableLevel;

            // Max tier reached
            if (currentTier >= tierCosts.Length)
            {
                buttonTexts[i].text = "Table "
                    + (i + 1) + " — MAX TIER";
                continue;
            }

            // Check level requirement for next tier
            int requiredLevel = tierLevelRequirements[currentTier];
            int cost = tierCosts[currentTier];

            if (playerLevel < requiredLevel)
            {
                buttonTexts[i].text = "Table " + (i + 1)
                    + " Tier " + (currentTier + 1)
                    + " — Need Level " + requiredLevel;
            }
            else
            {
                buttonTexts[i].text = "Upgrade Table "
                    + (i + 1)
                    + " → Tier " + (currentTier + 1)
                    + ": " + cost + " Coins";
            }
        }
    }

    // Call this when player levels up to refresh button states
    public void OnPlayerLevelUp()
    {
        UpdateUpgradeUI();
    }

    public bool[] GetTableUnlockStates()
    {
        bool[] states = new bool[tables.Length];
        for (int i = 0; i < tables.Length; i++)
        {
            states[i] = tables[i] != null ? tables[i].isUnlocked : false;
        }
        return states;
    }

    public int[] GetTableTiers()
    {
        int[] tiers = new int[tables.Length];
        for (int i = 0; i < tables.Length; i++)
        {
            tiers[i] = tables[i] != null ? tables[i].tableLevel : 1;
        }
        return tiers;
    }
}