using UnityEngine;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Concurrent;
using UnityEngine.AI;

public class DailyRewardManager : MonoBehaviour
{   
    public static DailyRewardManager Instance;

    [Header("UI References")]
    public GameObject dailyRewardPanel;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timerText;

    [Header("Reward Tracking")]
    public string lastClaimedDate = "";
    public int currentStreak = 0;

    private string[] rewardLabels = new string[]
    {
        "100 Coins",          // Day 1
        "Speed Booster",      // Day 2
        "Decoration Item",    // Day 3
        "5 Gems",             // Day 4
        "Rare Recipe",        // Day 5
        "Mystery Box",        // Day 6
        "Exclusive Furniture" // Day 7 
    };

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
        LoadDailyRewardData();

        if(dailyRewardPanel != null) dailyRewardPanel.SetActive(false);

        UpdateTimerUI();
    }


    //-- OPEN/CLOSE-----------------------------------
    public void OpenDailyReward()
    {
        if(dailyRewardPanel != null) dailyRewardPanel.SetActive(true);

        UpdateRewardUI();
    }

    public void CloseDailyReward()
    {
        if(dailyRewardPanel != null) dailyRewardPanel.SetActive(false);
    }

    //---CLAIM---------------------------------------------
    public void ClaimReward()
    {
        if(!CanClaim())
        {
            Debug.Log("Daily reward already claimed. Come back tomorrow!");
            return;
        }

        // check if streak should reset
        // if more than 48 hours passed, reset streak
        if (lastClaimedDate != "")
        {
            DateTime lastClaimed = DateTime.Parse(lastClaimedDate);
            double hoursSince = (DateTime.Now - lastClaimed).TotalHours;

            if (hoursSince > 48)
            {
                currentStreak = 0;
                Debug.Log("Streak reset - you have been away for a while.");
            }
        }

        // advance streak (loops back after day 7)
        currentStreak++;
        if(currentStreak > 7) currentStreak = 1;

        // give reward for current streak day
        GiveReward(currentStreak);

        //save claimed date
        lastClaimedDate = DateTime.Now.ToString();

        // add XP for claiming
        if (GameManager.Instance != null) GameManager.Instance.AddXP(50);

        // save everything
        SaveDailyRewardData();
        UpdateRewardUI();
        UpdateTimerUI();
    
        Debug.Log("Daily Reward claimed! Day " + currentStreak);
    }

    //--- REWARD LOGIC ----------------------------------------
    void GiveReward(int day)
    {
        switch (day)
        {
            case 1:
                GameManager.Instance.AddCoins(100);
                Debug.Log("reward: 100 Coins");
            break;

            case 2: 

                Debug.Log("Reward: Speed Booster activated");
            break;

            case 3:

                Debug.Log("Reward: Decoration Item granted");
            break;

            case 4:
                GameManager.Instance.AddGems(5);
                Debug.Log("Reward: 5 Gems");
            break;

            case 5:

                Debug.Log("Reward: Rare Recipe unloacked");
            break;

            case 6:

                GiveMysteryBox();
            break;

            case 7:

                Debug.Log("Reward: Exclusive Furniture granted!");
            break;
        }
    }

    void GiveMysteryBox()
    {
        int roll = UnityEngine.Random.Range(0,3);
        switch (roll)
        {
            case 0:
                GameManager.Instance.AddCoins(500);
                Debug.Log("Mystery Box: 500 Coins!");
            break;

            case 1: 
                GameManager.Instance.AddGems(10);
                Debug.Log("Mystery Box: 10 Gems!");
            break;

            case 2:
                GameManager.Instance.AddCoins(200);
                GameManager.Instance.AddGems(5);
                Debug.Log("Mystery Box: 200 Coins + Gems!");
            break;
        }
    }

    //---CLAIM CHECK--------------------------------------------
    public bool CanClaim()
    {
        if(lastClaimedDate == "") return true;

        DateTime lastClaimed = DateTime.Parse(lastClaimedDate);
        double hoursSince = (DateTime.Now - lastClaimed).TotalHours;
        return hoursSince >= 24;
    }

    //--- TIMER UNTIL NEXT CLAIM --------------------------------------
    TimeSpan GetTimeUntilNextClaim()
    {
        if(lastClaimedDate == "") return TimeSpan.Zero;

        DateTime lastClaimed = DateTime.Parse(lastClaimedDate);
        DateTime nextClaim = lastClaimed.AddHours(24);
        TimeSpan remaining = nextClaim - DateTime.Now;
        return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
    }

    //---UI--------------------------------------------------
    void UpdateRewardUI()
    {
        if (streakText != null) streakText.text = "Day " + currentStreak + " / 7";
        
        int nextDay = currentStreak >= 7 ? 1 : currentStreak + 1;

        if (rewardText != null) rewardText.text = "Next Reward: " + rewardLabels[Mathf.Clamp(nextDay - 1, 0, 6)];
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        if (CanClaim())
        {
            timerText.text = "Reward ready!";
        }
        else
        {
            TimeSpan t = GetTimeUntilNextClaim();
            timerText.text = string.Format("Next reward in {0:D2}:{1:D2}:{2:D2}",
                t.Hours, t.Minutes, t.Seconds);
        }
    }

    //--- SAVE/LOAD -----------------------------------
    void SaveDailyRewardData()
    {
        if (SaveSystem.Instance != null) SaveSystem.Instance.Save();
    }

    void LoadDailyRewardData()
    {
        if (SaveSystem.Instance == null) return;

        GameData data = SaveSystem.Instance.Load();
        lastClaimedDate = data.lastDailyRewardClaimed;
        currentStreak = data.dailyRewardStreak;
    }
}
