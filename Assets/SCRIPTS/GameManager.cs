using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int coins = 0;
    public int xp = 0;
    
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI xpText;

    private void Awake() { Instance = this; }

    private void Start() { UpdateUI(); }

    public void AddRewards(int coinAmount, int xpAmount)
    {
        coins += coinAmount;
        xp += xpAmount;
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount) {
            coins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + coins;
        xpText.text = "XP: " + xp;
    }
}