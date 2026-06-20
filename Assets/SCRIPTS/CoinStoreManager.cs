using UnityEngine;
using System.Collections;
using TMPro;

public class CoinStoreManager : MonoBehaviour
{
    public GemStoreManager gemStore;
    public GameObject coinStorePanel;
    public GameObject successPanel;
    public TextMeshProUGUI coinCounterText;

    private int currentCoins = 0;

    void Start()
    {
        UpdateCoinUI();
        if (coinStorePanel != null)
        {
            coinStorePanel.SetActive(false);
        }
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
    }

    public void OpenCoinStore()
    {
        if (coinStorePanel != null)
        {
            coinStorePanel.SetActive(true);
        }
    }

    public void CloseCoinStore()
    {
        if (coinStorePanel != null)
        {
            coinStorePanel.SetActive(false);
        }
    }

    public void BuyCoinPouch()
    {
        ProcessCoinPurchase(20, 500);
    }

    public void BuyPiggyBank()
    {
        ProcessCoinPurchase(80, 2000);
    }

    public void BuyCoinVault()
    {
        ProcessCoinPurchase(200, 6000);
    }

    private void ProcessCoinPurchase(int gemCost, int coinReward)
    {
        if (gemStore != null && gemStore.HasEnoughGems(gemCost))
        {
            gemStore.DeductGems(gemCost);
            currentCoins += coinReward;
            UpdateCoinUI();

            if (successPanel != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShowSuccessNotification());
            }
        }
        else
        {
            Debug.Log("Not enough gems to buy this coin bundle!");
        }
    }

    IEnumerator ShowSuccessNotification()
    {
        successPanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        successPanel.SetActive(false);
    }

    private void UpdateCoinUI()
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = "Coins: " + currentCoins.ToString();
        }
    }
}