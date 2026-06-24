using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class AnimalStarsManager : MonoBehaviour
{
    public static AnimalStarsManager Instance;

    [Header("Star Rating Settings")]
    public float animalStars = 1.0f;
    private const float MinStars = 1.0f;
    private const float MaxStars = 5.0f;

    [Header("Optional UI")]
    public TextMeshProUGUI starsText;

    [Header("Streak Tracking")]
    private int currentServingStreak = 0;

    [Header("Passive Recovery")]
    private float passiveTimer = 0f;
    private const float RecoveryInterval = 90f;
    private const float PassiveReward = 0.25f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance =  this;
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
        // active play grants +0.25 stars every 90 seconds
        passiveTimer += Time.deltaTime;
        if(passiveTimer >= RecoveryInterval)
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

        if(currentServingStreak == 5)
        {
            AddStars(0.25f);
            Debug.Log("Streak Milestone! 5 customers served (+0.25 Stars).");
        }
        else if (currentServingStreak == 10)
        {
            AddStars(0.50f);
            Debug.Log("Streak Milestone! 10 customers served (+0.50 Stars). Resetting streak counter");
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
        return 0.4f; // for 1.0 - 1.4 stars
    }

    // called by customerspawner to slowdown if reputation drops
    public float GetSpawnRateMultiplier()
    {
        // under 2.5 stars, customer arrival delays 
        if(animalStars < 2.5f)
        {
            return 0.6f; // runs 60% of normal efficiency
        }
        return 1.0f; // Normal operation speed
    }

    //--- SYSTEM ARCHITECTURE-------------------------------------------\
    void UpdateUI()
    {
        if (starsText != null)
        {
            starsText.text = "Stars " + animalStars.ToString("F1") + " ⭐";
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

            if (data.animalStars >= MinStars && data.animalStars <= MaxStars)
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
