using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public enum RewardType
{
    Coins,
    Gems,
    XP,
    SpeedBooster // Instant cook boost for 5 minutes
}

[System.Serializable]
public class DailyReward
{
    public int dayNumber;          // 1 to 7
    public RewardType rewardType;  
    public int amount;             
}

public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance;

    [Header("7-Day Reward Config")]
    public DailyReward[] rewards = new DailyReward[7];

    [Header("UI Pop-up Elements")]
    public GameObject rewardPanel;        
    public TMP_Text rewardNotificationText;
    public Button closeButton;            

    [Header("Day Item Visual Elements (Size = 7)")]
    public GameObject[] daySlots;         
    public TMP_Text[] rewardAmountTexts;  
    public GameObject[] claimedOverlays;  

    // Added these public fields so SaveSystem.cs can access them without CS1061 errors!
    [HideInInspector] public string lastClaimedDate = "";
    [HideInInspector] public int currentStreak = 0;

    private const string LAST_CLAIM_KEY = "LastClaimDate";
    private const string STREAK_KEY = "RewardStreakIndex";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

    lastClaimedDate = PlayerPrefs.GetString(LAST_CLAIM_KEY, "");
    currentStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);

    UpdateUIVisuals(currentStreak);

    CheckAndAutoClaim();
   }

    // Added method so MainMenuButtonScripts.cs can call it without CS1061 error!
    public void OpenDailyReward()
    {
        OpenPanel();
    }

    public void CheckAndAutoClaim()
    {
        if (CanClaimToday())
        {
            int streak = GetCurrentStreakIndex();
            DailyReward rewardToClaim = rewards[streak];

            // Grant the reward
            GrantReward(rewardToClaim);

            // ── FIXED: Plays daily reward sound upon successful item claim ─────
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayDailyReward();
            }

            // Save date & streak
            lastClaimedDate = DateTime.Now.Date.ToString("o");
            PlayerPrefs.SetString(LAST_CLAIM_KEY, lastClaimedDate);

            int nextStreak = (streak + 1) % 7;
            currentStreak = nextStreak;
            PlayerPrefs.SetInt(STREAK_KEY, nextStreak);
            PlayerPrefs.Save();

            // Display text in UI
            if (rewardNotificationText != null)
            {
                if (rewardToClaim.rewardType == RewardType.SpeedBooster)
                {
                    rewardNotificationText.text = $"DAY {streak + 1} REWARD CLAIMED!\n+5 MIN SPEED BOOSTER";
                }
                else
                {
                    rewardNotificationText.text = $"DAY {streak + 1} REWARD CLAIMED!\n+{rewardToClaim.amount} {rewardToClaim.rewardType}";
                }
            }

            UpdateUIVisuals(nextStreak == 0 ? 7 : nextStreak);
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    public bool CanClaimToday()
    {
        if (!PlayerPrefs.HasKey(LAST_CLAIM_KEY)) return true;

        string lastClaimString = PlayerPrefs.GetString(LAST_CLAIM_KEY);
        DateTime lastClaimDate;
        if (!DateTime.TryParse(lastClaimString, out lastClaimDate)) return true;

        DateTime today = DateTime.Now.Date;
        return today > lastClaimDate;
    }

    private int GetCurrentStreakIndex()
    {
        if (!PlayerPrefs.HasKey(LAST_CLAIM_KEY)) return 0;

        string lastClaimString = PlayerPrefs.GetString(LAST_CLAIM_KEY);
        DateTime lastClaimDate;
        if (!DateTime.TryParse(lastClaimString, out lastClaimDate)) return 0;

        DateTime today = DateTime.Now.Date;

        if ((today - lastClaimDate).Days > 1)
        {
            currentStreak = 0;
            PlayerPrefs.SetInt(STREAK_KEY, 0);
            return 0;
        }

        currentStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);
        return currentStreak;
    }

    public void UpdateUIVisuals(int activeStreakIndex)
    {
        for (int i = 0; i < 7; i++)
        {
            if (rewardAmountTexts != null && i < rewardAmountTexts.Length && rewardAmountTexts[i] != null)
            {
                if (i < rewards.Length && rewards[i] != null)
                {
                    if (rewards[i].rewardType == RewardType.SpeedBooster)
                        rewardAmountTexts[i].text = "Speed Booster";
                    else
                        rewardAmountTexts[i].text = $"{rewards[i].amount} {rewards[i].rewardType}";
                }
            }

            if (claimedOverlays != null && i < claimedOverlays.Length && claimedOverlays[i] != null)
            {
                claimedOverlays[i].SetActive(i < activeStreakIndex);
            }
        }
    }

    private void GrantReward(DailyReward reward)
    {
        switch (reward.rewardType)
        {
            case RewardType.Coins:
                if (GameManager.Instance != null)
                {
                    // Add coins if applicable
                }
                break;

            case RewardType.XP:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddXP(reward.amount);
                }
                break;

            case RewardType.SpeedBooster:
                if (SpeedBoosterManager.Instance != null)
                {
                    SpeedBoosterManager.Instance.ActivateSpeedBooster(300f); // 300 seconds (5 mins)
                }
                break;
        }

        Debug.Log($"AUTO-CLAIMED Day {reward.dayNumber}: {reward.rewardType}");
    }

    public void OpenPanel()
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);

        // ── FIXED: Plays a panel open clip when dashboard displays ───────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelOpen();
        }
    }

    public void ClosePanel()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);

        // ── FIXED: Plays a panel close clip when dashboard dismisses ─────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelClose();
        }
    }
}
