using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Panel")]
    public GameObject recipePanel;

    [Header("Recipe Cards — assign 5 cards in order")]
    public Image[]            cardBGs;
    public Image[]            foodImages;
    public TextMeshProUGUI[]  nameTexts;
    public TextMeshProUGUI[]  statusTexts;

    [Header("Food Sprites — assign in order")]
    public Sprite[] foodSprites;

    private string[] recipeNames = new string[]
    {
        "Coffee",
        "Tea",
        "Muffin",
        "Pancakes",
        "Special Pawfee"
    };

    private int[] recipeUnlockLevels = new int[]
    {
        1,   // Coffee
        2,   // Tea
        3,   // Muffin
        5,   // Pancakes
        7    // Special Pawfee
    };

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
    }

    // --- OPEN / CLOSE -------------------------
    public void OpenRecipePanel()
    {
        if (recipePanel != null)
            recipePanel.SetActive(true);

        RefreshRecipeUI();
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
}