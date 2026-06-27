using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data.Common;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Main Panel")]
    public GameObject recipePanel;

    [Header("Recipe Cards")]
    public Image[]            cardBGs;
    public Image[]            foodImages;
    public TextMeshProUGUI[]  nameTexts;
    public TextMeshProUGUI[]  statusTexts;

    [Header("Top Info Area")]
    public Image infoFoodImage;
    public TextMeshProUGUI infoDescriptionText;
    public TextMeshProUGUI infoStatsText;
    public GameObject infoLearnButton;

    [Header("Food Sprites")]
    public Sprite[] foodSprites;
    private string[] recipeNames = new string[] {"Coffee","Tea","Muffin","Pancakes","Special Pawfee"};
    private int[] recipeUnlockLevels = new int[]{1, 2, 3, 5, 7};
    private int[] recipePrepTimes = new int[]{5, 6, 8, 10, 12 };
    private int[] recipeCoinRewards = new int[]{20, 25, 35,50,75};
    private int[] recipePurchaseCosts = new int[] { 100, 800, 6000, 45000, 30000};
    
    private bool[] isRecipePurchased = new bool[] {true, false, false, false,false};
    private int currentlySelectedRecipeIndex = 0;
    
    // Card colors
    private Color unlockedColor  = new Color(0.84f, 0.97f, 0.88f); // soft green
    private Color nextColor      = new Color(1.0f,  0.97f, 0.84f); // soft amber
    private Color lockedColor    = new Color(0.90f, 0.90f, 0.90f); // light grey

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (recipePanel != null)
            recipePanel.SetActive(false);

        if (SaveSystem.Instance != null)
        {
            GameData loadedData = SaveSystem.Instance.Load();
            if (loadedData != null && loadedData.recipeUnlocks != null && loadedData.recipeUnlocks.Length == isRecipePurchased.Length)
            {
                isRecipePurchased = loadedData.recipeUnlocks;
            }
            
        }
    }

    // --- OPEN / CLOSE -------------------------
    public void OpenRecipePanel()
    {
        if (recipePanel != null)
            recipePanel.SetActive(true);

        RefreshRecipeUI();
        SelectRecipeCard(0);
    }

    public void CloseRecipePanel()
    {
        if (recipePanel != null)
            recipePanel.SetActive(false);
    }

    //--- UI REFRESH ---------------------------
    public void RefreshRecipeUI()
    {
        int playerLevel = LevelSystem.Instance != null
            ? LevelSystem.Instance.currentLevel : 1;

        for (int i = 0; i < recipeNames.Length; i++)
        {
            bool isUnlocked = playerLevel >= recipeUnlockLevels[i];
            bool isNext = false;

            if (!isUnlocked)
            {
                if (i == 0 || playerLevel >= recipeUnlockLevels[i - 1])
                {
                    isNext = true;
                    for (int prev = 0; prev < i; prev++) 
                    {
                        if (playerLevel < recipeUnlockLevels[prev]) 
                        {
                            isNext = false;
                        }
                    }
                }
            }

            // Card name
            if (i < foodImages.Length && foodImages[i] != null)
            {
                if (i < foodSprites.Length && foodSprites[i] != null)
                {
                    foodImages[i].sprite = foodSprites[i];
                }
                else
                {
                    foodImages[i].sprite = null; // Keeps it blank safely if art isn't ready
                }

                // Grey out locked recipes (Runs perfectly now even without art assets assigned)
                foodImages[i].color = isUnlocked
                    ? Color.white
                    : new Color(0.6f, 0.6f, 0.6f, 1f);
            }

            // Status text
            if (i < statusTexts.Length && statusTexts[i] != null)
            {
                if (isUnlocked)
                    statusTexts[i].text = "✓";
                else
                    statusTexts[i].text = "Level " + recipeUnlockLevels[i];
            }

            // Card background color
            if (i < cardBGs.Length && cardBGs[i] != null)
            {
                if (isUnlocked)      cardBGs[i].color = unlockedColor;
                else if (isNext)     cardBGs[i].color = nextColor;
                else                 cardBGs[i].color = lockedColor;
            }
        }
    }

        //---CLICK SELECTION TRIGGERED BY CARDS
    public void SelectRecipeCard(int index)
    {
        currentlySelectedRecipeIndex = index;

        // top are illustration
        if (infoFoodImage != null)
        {
            if (index < foodSprites.Length && foodSprites[index] != null)
            {
                infoFoodImage.sprite = foodSprites[index];
                infoFoodImage.gameObject.SetActive(true);
            }
            else
            {
                infoFoodImage.gameObject.SetActive(false); // Hide if art asset isn't ready
            }
        }

        // Set description text string matching selection name
        if (infoDescriptionText != null)
        {
            infoDescriptionText.text = "A delicious premium item served fresh at Pawfee Corner.";
        }

        // Render Level Progress and Coin stats rules line by line
        if (infoStatsText != null)
        {
            int currentLevel = LevelSystem.Instance != null ? LevelSystem.Instance.currentLevel : 1;
            bool meetsLevel = currentLevel >= recipeUnlockLevels[index];
            bool alreadyOwned = isRecipePurchased[index];

            string statsOutput = "🐾 Prep time: " + recipePrepTimes[index] + "s\n" +
                                  "🐾 Reward: +" + recipeCoinRewards[index] + " Coins\n";

            if (alreadyOwned)
            {
                statsOutput += "<color=green>✨ Status: Unlocked & Active!</color>";
                if (infoLearnButton != null) infoLearnButton.SetActive(false);
            }
            else
            {
                string validationMark = meetsLevel ? "<color=green>✓ Ready to learn</color>" : "<color=red>❌ Locked: Requires Level " + recipeUnlockLevels[index] + "</color>";
                statsOutput += validationMark + "\nCost: " + recipePurchaseCosts[index] + " Coins";
                
                // Show click button to buy only if level tier requirements are met
                if (infoLearnButton != null) infoLearnButton.SetActive(meetsLevel);
            }

            infoStatsText.text = statsOutput;
        }
    }

    //---PURCHASE LOGIC CALLED BY AVAIL RECIPE BUTTON--------------
    public void BuySelectedRecipe()
    {
        int index = currentlySelectedRecipeIndex;

        // Safety check
        if (index < 0 || index >= recipeNames.Length) return;

        // If bought, do nothing
        if (isRecipePurchased[index]) return;

        int cost = recipePurchaseCosts[index];

        // 1. Verify if your GameManager has enough coins to pay for it
        if (GameManager.Instance != null && GameManager.Instance.coins >= cost)
        {
            // 2. Deduct the currency from your GameManager data
            GameManager.Instance.coins -= cost;

            // 3. Mark the recipe index as permanently unlocked
            isRecipePurchased[index] = true;

            // 4. Force all UI components to redraw with the updated unlock state
            RefreshRecipeUI();
            SelectRecipeCard(index);

            Debug.Log("Successfully learned recipe: " + recipeNames[index]);
            
            if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
        }
        else
        {
            Debug.Log("Not enough Coins to buy " + recipeNames[index] + "! Missing: " + (cost - (GameManager.Instance != null ? GameManager.Instance.coins : 0)));
        }
    }

    //--- DATA EXPORTER ACCESSED BY THE SAVE SYSTEM ------------------
    public bool[] GetRecipeUnlockStates()
    {
        return isRecipePurchased;
    }

}




