using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimalStarsManager : MonoBehaviour
{
    public static AnimalStarsManager Instance;

    [Header("Star Rating Settings")]
    public float animalStars = 1.0f;
    private const float MinStars = 1.0f;
    private const float MaxStars = 5.0f;

    [Header("Optional UI")]
    public TextMeshProUGUI starsText;

    // ── FINISHED: INTEGRATED PLAYTEST VISUAL SPRITE ASSETS ──────────────────
    [Header("Visual Star Configuration")]
    [Tooltip("Drag Star_0 through Star_4 here from your Hierarchy in order")]
    public Image[] starImages;            
    public Sprite filledStarSprite;       // Drag your '5 stars filled' art asset here
    public Sprite blankStarSprite;        // Drag your '5 stars blank' art asset here

    [Header("Streak Tracking")]
    private int currentServingStreak = 0;

    [Header("Passive Recovery")]
    private float passiveTimer = 0f;
    private const float RecoveryInterval = 75f;
    private const float PassiveReward = 0.25f;

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
    }
  
    private void Start()
    {
        LoadSavedStars();
        UpdateUI();
    }

    void Update()
    {
        // active play grants +0.25 stars every 75 seconds
        passiveTimer += Time.deltaTime;
        if (passiveTimer >= RecoveryInterval)
        {
            passiveTimer = 0f;
            AddStars(PassiveReward);
            Debug.Log("Animal Stars: Passive Active play recovery granted (+0.25).");
        }
    }

    //---PUBLIC MODIFIERS---------------------------------------------
    public void AddStars(float amount)
    {
        animalStars = Mathf.Clamp(animalStars + amount, MinStars, MaxStars);
        UpdateUI();
        SaveGameData();
    }

    public void RemoveStars(float amount)
    {
        animalStars = Mathf.Clamp(animalStars - amount, MinStars, MaxStars);
        UpdateUI();
        SaveGameData();

        // reset serving streak on penalty failure
        currentServingStreak = 0;
    }

    // track serving streaks to reward players
    public void RecordSuccessfulService()
    {
        currentServingStreak++;

        if (currentServingStreak == 5)
        {
            float bonus = animalStars < 2.5f ? 0.15f : 0.25f;
            AddStars(0.25f);
            Debug.Log("Streak: 5 customers served (+ " + bonus + " 0.25 Stars).");
        }
        else if (currentServingStreak == 10)
        {   
            float bonus = animalStars < 2.5f ? 0.25f : 0.5f;
            AddStars(0.50f);
            Debug.Log("Streak: ! 10 customers served (" + bonus + " 0.50 Stars). Resetting streak counter");
            currentServingStreak = 0;
        }
    }

    //--- REWARD MULTIPLIERS-------------------------------------------------
    public float GetCoinMultiplier()
    {
        if (animalStars >= 4.5f) return 1.5f;
        if (animalStars >= 3.5f) return 1.2f;
        if (animalStars >= 2.5f) return 1.0f;
        if (animalStars >= 1.5f) return 0.7f;
        return 0.8f; // for 1.0 - 1.4 stars
    }

    // called by customerspawner to slowdown if reputation drops
    public float GetSpawnRateMultiplier()
    {
        if (animalStars < 1.5f) return 0.6f;
        return 1.0f; 
    }

    //--- SYSTEM ARCHITECTURE-------------------------------------------
    void UpdateUI()
    {
        // 1. Maintain your original text display formatting strings safely
        if (starsText != null)
        {
            starsText.text = "Stars " + animalStars.ToString("F1") + " ⭐";
        }  

        // 2. Dynamically calculate fractional fill values for each star box
        if (starImages == null || starImages.Length == 0) return;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                float targetStarBaseline = i + 1f;

                if (animalStars >= targetStarBaseline)
                {
                    starImages[i].fillAmount = 1f;
                }
                else if (animalStars > targetStarBaseline - 1f)
                {
                    float remainderFraction = animalStars - (targetStarBaseline - 1f);
                    starImages[i].fillAmount = Mathf.Clamp01(remainderFraction);
                }
                else
                {
                    starImages[i].fillAmount = 0f;
                }
            }
        }
    }

    void SaveGameData()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }
    }
    
    void LoadSavedStars()
    {
        if (SaveSystem.Instance != null)
        {
            GameData data = SaveSystem.Instance.Load();

            if (data != null && data.animalStars >= MinStars && data.animalStars <= MaxStars) 
            {
                animalStars = data.animalStars;
            }
            else
            {
                animalStars = 1.0f;
            }
        }
    }
}