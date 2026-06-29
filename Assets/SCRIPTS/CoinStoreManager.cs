using UnityEngine;
using System.Collections;

public class CoinStoreManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject coinStorePanel;
    public GameObject successPanel;

    void Start()
    {
        if (coinStorePanel != null) coinStorePanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    public void OpenCoinStore()
    {
        if (coinStorePanel != null) coinStorePanel.SetActive(true);
    }

    public void CloseCoinStore()
    {
        if (coinStorePanel != null) coinStorePanel.SetActive(false);
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
        if (GameManager.Instance.SpendGems(gemCost))
        {
            GameManager.Instance.AddCoins(coinReward);

            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.Save();
            }

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
}