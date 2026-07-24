using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Main Panel Reference")]
    public GameObject recipePanel;

    [Header("Grid Cards — assign 5 elements in order")]
    public Image[]            cardBGs;      
    public Image[]            foodImages;   
    public TextMeshProUGUI[]  nameTexts;
    public TextMeshProUGUI[]  statusTexts;  

    [Header("Combined Top Info Area Layout")]
    public Image infoFoodImage;                 
    public TextMeshProUGUI infoDescriptionText; 
    public TextMeshProUGUI infoStatsText;       
    public GameObject infoLearnButton;          

    [Header("Food Sprites Art Assets — assign in order")]
    public Sprite[] foodSprites;

    // Data profiles built from GDD rules
    private string[] recipeNames = new string[] { "Coffee", "Tea", "Muffin", "Pancakes", "Special Pawfee" };
    private int[] recipeUnlockLevels = new int[] { 1, 2, 3, 5, 7 }; 
    private int[] recipePrepTimes = new int[] { 5, 6, 8, 10, 12 };  
    private int[] recipeCoinRewards = new int[] { 20, 25, 35, 50, 75 }; 
    private int[] recipePurchaseCosts = new int[] { 100, 800, 6000, 45000, 300000 }; 

    // ── FIXED: Added unique descriptions for every recipe from your GDD ──
    private string[] recipeDescriptions = new string[]
    {
        "A warm cup of freshly brewed black coffee. Simple and full of caffeine.",
        "Calming green tea leaves steeped to perfection. A rustic cafe classic.",
        "A sweet chocolate chip muffin baked fresh this morning. Super fluffy!",
        "Fluffy golden pancakes stacked high and drenched in sweet maple syrup.",
        "Our signature house specialty premium blend. With a hint of catnip for that extra kick!"
    };

    private bool[] isRecipePurchased = new bool[] { true, false, false, false, false }; 
    private int currentlySelectedRecipeIndex = 0; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (recipePanel != null)
            recipePanel.SetActive(false);

        // Load data from file
        if (SaveSystem.Instance != null)
        {
            GameData loadedData = SaveSystem.Instance.Load();
            if (loadedData != null && loadedData.recipeUnlocks != null && loadedData.recipeUnlocks.Length == isRecipePurchased.Length)
            {
                isRecipePurchased = loadedData.recipeUnlocks;
            }
        }
    }

    public void OpenRecipePanel()
    {
        if (recipePanel != null)
            recipePanel.SetActive(true);

        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelOpen();

        RefreshRecipeUI();
        SelectRecipeCard(0); 
    }

    public void CloseRecipePanel()
    {
        if (recipePanel != null)
            recipePanel.SetActive(false);

        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelClose();

    }

    public void RefreshRecipeUI()
    {
        int playerLevel = LevelSystem.Instance != null ? LevelSystem.Instance.currentLevel : 1;

        for (int i = 0; i < recipeNames.Length; i++)
        {
            bool levelRequirementMet = playerLevel >= recipeUnlockLevels[i];
            bool alreadyOwned = isRecipePurchased[i];

            if (i < nameTexts.Length && nameTexts[i] != null)
                nameTexts[i].text = recipeNames[i];

            if (i < foodImages.Length && foodImages[i] != null)
            {
                if (i < foodSprites.Length && foodSprites[i] != null)
                    foodImages[i].sprite = foodSprites[i];
                else
                    foodImages[i].sprite = null;

                foodImages[i].color = alreadyOwned
                    ? Color.white
                    : new Color(0.15f, 0.15f, 0.15f, 1f); 
            }

            // ── FIXED: Removed all emoji characters that print as broken box symbols ──
            if (i < statusTexts.Length && statusTexts[i] != null)
            {
                if (alreadyOwned)
                    statusTexts[i].text = "Owned"; 
                else if (levelRequirementMet)
                    statusTexts[i].text = recipePurchaseCosts[i].ToString(); 
                else
                    statusTexts[i].text = "Level " + recipeUnlockLevels[i]; 
            }
        }
    }

    public void SelectRecipeCard(int index)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();

        currentlySelectedRecipeIndex = index;
        
        int currentLevel = LevelSystem.Instance != null ? LevelSystem.Instance.currentLevel : 1;
        bool meetsLevel = currentLevel >= recipeUnlockLevels[index];
        bool alreadyOwned = isRecipePurchased[index];

        if (infoFoodImage != null)
        {
            if (index < foodSprites.Length && foodSprites[index] != null)
            {
                infoFoodImage.sprite = foodSprites[index];
                infoFoodImage.gameObject.SetActive(true);
                infoFoodImage.color = Color.white; 
            }
            else
            {
                infoFoodImage.gameObject.SetActive(false);
            }
        }

        // ── FIXED: Updates to pull the specific description matching the item clicked ──
        if (infoDescriptionText != null && index < recipeDescriptions.Length)
        {
            infoDescriptionText.text = recipeDescriptions[index];
        }

        if (infoStatsText != null)
        {
            string statsTextContent = "Prep time: " + recipePrepTimes[index] + "s\n" +
                                      "Rewards: +" + recipeCoinRewards[index] + " Coins\n";

            if (alreadyOwned)
                statsTextContent += "<color=green>Status: Unlocked</color>";
            else if (meetsLevel)
                statsTextContent += "<color=yellow>Ready to learn.</color>";
            else
                statsTextContent += "<color=red>Locked: Level " + recipeUnlockLevels[index] + "</color>";

            infoStatsText.text = statsTextContent;
        }

        if (infoLearnButton != null)
        {
            if (alreadyOwned)
            {
                infoLearnButton.SetActive(false); 
            }
            else
            {
                infoLearnButton.SetActive(meetsLevel); 
                TMP_Text buttonLabel = infoLearnButton.GetComponentInChildren<TMP_Text>();
                if (buttonLabel != null)
                {
                    buttonLabel.text = "Learn";
                }
            }
        }

        for (int i = 0; i < cardBGs.Length; i++)
        {
            if (cardBGs[i] != null)
            {
                Outline cardOutline = cardBGs[i].GetComponent<Outline>();
                if (cardOutline != null)
                {
                    cardOutline.enabled = (i == index);
                }
            }
        }
    }

    public void BuySelectedRecipe()
    {
        int index = currentlySelectedRecipeIndex;
        if (index < 0 || index >= recipeNames.Length || isRecipePurchased[index]) return;

        int cost = recipePurchaseCosts[index];

        if (GameManager.Instance != null && GameManager.Instance.coins >= cost)
        {
            GameManager.Instance.coins -= cost;
            isRecipePurchased[index] = true;

            RefreshRecipeUI();
            SelectRecipeCard(index);

            if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
        }
    }

    public bool[] GetRecipeUnlockStates()
    {
        return isRecipePurchased;
    }
}
