using UnityEngine;
using UnityEngine.UI; // Needed for Image component
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Table Object References")]
    public CafeTable[] tables;

    [Header("Buy Panel Text Display Area")]
    public TMP_Text[] buyButtonTexts; 

    [Header("Custom UI Button Modifiers")]
    public Image[] buyButtonImages;
    public Sprite checkmarkButtonSprite;
    public Sprite normalButtonSprite;

    // ADDED: Your new custom UI modifiers for the Upgrade Panel
    [Header("Custom Upgrade UI Button Modifiers")]
    public Image[] upgradeButtonImages;
    public Sprite notOwnedUpgradeButtonSprite;
    public Sprite normalUpgradeButtonSprite;
    public Sprite maxUpgradeButtonSprite;

    [Header("Upgrade Panel Text Display Area")]
    public TMP_Text[] upgradeButtonTexts; 

    [Header("Description Text Display Area")]
    public TMP_Text[] descriptionTexts; 

    [Header("Buy Page Level Lock Overlays")]
    public GameObject[] buyPageLockOverlays; 

    [Header("Upgrade Page Level Lock Overlays")]
    public GameObject[] upgradePageLockOverlays; 

    [Header("Top Header Navigation UI Text References")]
    public TMP_Text pageTitleText;

    [Header("Main Content Panel Screen References")]
    public GameObject buyPagePanel;   
    public GameObject upgradePagePanel; 

    private int currentPageIndex = 0; // 0 = Buy/Unlock Page, 1 = Upgrade Page

    private int[] tierCosts = new int[]
    {
        150,     
        1200,    
        9000,    
        70000,   
        500000   
    };

    private int[] tierLevelRequirements = new int[]
    {
        1,  
        3,  
        5,  
        7,  
        9   
    };

    private int[] unlockCosts = new int[]
    {
        0,    
        150,  
        300,  
        500,  
        800   
    };

    private int[] tableUnlockLevelRequirements = new int[]
    {
        1, 
        3, 
        5, 
        7, 
        9  
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
            
            if (data != null && data.tableUnlockedStates != null && data.tableUnlockedStates.Length == tables.Length)
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    if (tables[i] != null)
                    {
                        tables[i].isUnlocked = data.tableUnlockedStates[i];
                        tables[i].tableLevel = data.tableTiers[i];
                        
                        tables[i].gameObject.SetActive(data.tableUnlockedStates[i]);
                    }
                }
            }
        }
        
        UpdateUpgradeUI();
    }

    public void SwitchToBuyPage()
    {
        currentPageIndex = 0;
        UpdateUpgradeUI();
    }

    public void SwitchToUpgradePage()
    {
        currentPageIndex = 1;
        UpdateUpgradeUI();
    }

    public void OnClickUpgradeRow(int index)
    {
        if (index < 0 || index >= tables.Length) return;

        CafeTable targetTable = tables[index];
        int playerLevel = LevelSystem.Instance != null ? LevelSystem.Instance.currentLevel : 1;

        if (currentPageIndex == 0)
        {
            if (!targetTable.isUnlocked)
            {
                int requiredLevel = tableUnlockLevelRequirements[index];
                if (playerLevel < requiredLevel)
                {
                    Debug.LogWarning($"Cannot buy Table {index + 1}! Requires Level {requiredLevel}.");
                    return;
                }

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
                    Debug.Log("not enough coins to unlock table " + (index + 1));
                }
            }
            return;
        }

        if (currentPageIndex == 1)
        {
            if (!targetTable.isUnlocked) return;

            int currentTier = targetTable.tableLevel;
            if (currentTier >= tierCosts.Length) return;

            int requiredLevel = tierLevelRequirements[currentTier];
            if (playerLevel < requiredLevel) return;

            int cost = tierCosts[currentTier];
            if (GameManager.Instance.SpendCoins(cost))
            {
                targetTable.tableLevel++;
                GameManager.Instance.AddXP(25);
                UpdateUpgradeUI();

                if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
            }
        }
    }

    void UpdateUpgradeUI()
    {
        int playerLevel = LevelSystem.Instance != null ? LevelSystem.Instance.currentLevel : 1;

        if (pageTitleText != null)
        {
            pageTitleText.text = (currentPageIndex == 0) ? "buy tables" : "upgrade tiers";
        }

        if (buyPagePanel != null) buyPagePanel.SetActive(currentPageIndex == 0);
        if (upgradePagePanel != null) upgradePagePanel.SetActive(currentPageIndex == 1);

        for (int i = 0; i < tables.Length; i++)
        {
            CafeTable table = tables[i];
            if (table == null) continue;

            bool hasBuyLock = buyPageLockOverlays != null && i < buyPageLockOverlays.Length && buyPageLockOverlays[i] != null;
            bool hasUpgradeLock = upgradePageLockOverlays != null && i < upgradePageLockOverlays.Length && upgradePageLockOverlays[i] != null;
            bool hasDesc = descriptionTexts != null && i < descriptionTexts.Length && descriptionTexts[i] != null;

            if (i < buyButtonTexts.Length && buyButtonTexts[i] != null)
            {
                if (!table.isUnlocked)
                {
                    buyButtonTexts[i].text = $"unlock: {unlockCosts[i]} coins";
                    if (hasDesc && currentPageIndex == 0) 
                        descriptionTexts[i].text = $"purchase table {i + 1} to expand your shop seating capacity.";

                    if (buyButtonImages != null && i < buyButtonImages.Length && buyButtonImages[i] != null && normalButtonSprite != null)
                    {
                        buyButtonImages[i].sprite = normalButtonSprite;
                    }
                }
                else
                {
                    buyButtonTexts[i].text = "already owned";
                    if (hasDesc && currentPageIndex == 0) 
                        descriptionTexts[i].text = "table is unlocked! swap to the upgrade menu to increase its tiers.";

                    if (buyButtonImages != null && i < buyButtonImages.Length && buyButtonImages[i] != null && checkmarkButtonSprite != null)
                    {
                        buyButtonImages[i].sprite = checkmarkButtonSprite;
                    }
                }
            }

            if (i < upgradeButtonTexts.Length && upgradeButtonTexts[i] != null)
            {
                if (!table.isUnlocked)
                {
                    upgradeButtonTexts[i].text = "not owned yet";
                    if (hasDesc && currentPageIndex == 1) 
                        descriptionTexts[i].text = "unlock this table in the buy panel layout first.";

                    // ADDED: Swap to 'not owned yet' lock button UI sprite
                    if (upgradeButtonImages != null && i < upgradeButtonImages.Length && upgradeButtonImages[i] != null && notOwnedUpgradeButtonSprite != null)
                    {
                        upgradeButtonImages[i].sprite = notOwnedUpgradeButtonSprite;
                    }
                }
                else
                {
                    int currentTier = table.tableLevel;

                    if (currentTier >= tierCosts.Length)
                    {
                        upgradeButtonTexts[i].text = "max tier";
                        if (hasDesc && currentPageIndex == 1) 
                            descriptionTexts[i].text = $"tier {currentTier} (max)\nincome: +{currentTier} coins";

                        // ADDED: Swap to green 'max level' button UI sprite
                        if (upgradeButtonImages != null && i < upgradeButtonImages.Length && upgradeButtonImages[i] != null && maxUpgradeButtonSprite != null)
                        {
                            upgradeButtonImages[i].sprite = maxUpgradeButtonSprite;
                        }
                    }
                    else
                    {
                        int cost = tierCosts[currentTier];
                        upgradeButtonTexts[i].text = $"upgrade: {cost} coins";
                        
                        if (hasDesc && currentPageIndex == 1) 
                            descriptionTexts[i].text = $"tier {currentTier} -> tier {currentTier + 1}\nincome: +{currentTier} -> +{currentTier + 1} coins";

                        // ADDED: Swap to standard active upgrade button UI sprite
                        if (upgradeButtonImages != null && i < upgradeButtonImages.Length && upgradeButtonImages[i] != null && normalUpgradeButtonSprite != null)
                        {
                            upgradeButtonImages[i].sprite = normalUpgradeButtonSprite;
                        }
                    }
                }
            }

            // Manage Buy Page Lock UI States
            if (hasBuyLock)
            {
                if (table.isUnlocked)
                {
                    buyPageLockOverlays[i].SetActive(false); 
                }
                else
                {
                    int requiredUnlockLevel = tableUnlockLevelRequirements[i];
                    buyPageLockOverlays[i].SetActive(playerLevel < requiredUnlockLevel); 
                }
            }

            // Manage Upgrade Page Lock UI States
            if (hasUpgradeLock)
            {
                if (!table.isUnlocked)
                {
                    upgradePageLockOverlays[i].SetActive(true);
                }
                else
                {
                    int currentTier = table.tableLevel;
                    if (currentTier >= tierCosts.Length)
                    {
                        upgradePageLockOverlays[i].SetActive(false);
                    }
                    else
                    {
                        int requiredLevel = tierLevelRequirements[currentTier];
                        upgradePageLockOverlays[i].SetActive(playerLevel < requiredLevel);
                    }
                }
            }
        }
    }

    public void OnPlayerLevelUp()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayLevelUp();

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