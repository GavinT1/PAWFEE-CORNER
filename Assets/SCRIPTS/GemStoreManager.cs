using UnityEngine;
using System.Collections;
using TMPro;

public class GemStoreManager : MonoBehaviour
{
    public GameObject gemStorePanel;       
    public GameObject successPanel;        
    public TextMeshProUGUI gemCounterText; 

    private int currentGems = 0;

    void Start()
    {
        UpdateGemUI();
        if (gemStorePanel != null)
        {
            gemStorePanel.SetActive(false);
        }
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
    }

    public void OpenGemStore()
    {
        if (gemStorePanel != null)
        {
            gemStorePanel.SetActive(true);
        }
    }

    public void CloseGemStore()
    {
        if (gemStorePanel != null)
        {
            gemStorePanel.SetActive(false);
        }
    }

    public void BuyStarterBundle()
    {
        ProcessFakePurchase(50);
    }

    public void BuyPopularBundle()
    {
        ProcessFakePurchase(120);
    }

    public void BuyBestValueBundle()
    {
        ProcessFakePurchase(350);
    }

    public void BuyMegaBundle()
    {
        ProcessFakePurchase(800);
    }

    public bool HasEnoughGems(int amount)
    {
        return currentGems >= amount;
    }

    public void DeductGems(int amount)
    {
        currentGems -= amount;
        UpdateGemUI();
    }

    private void ProcessFakePurchase(int gemAmount)
    {
        currentGems += gemAmount;
        UpdateGemUI();

        if (successPanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowSuccessNotification());
        }
    }

    IEnumerator ShowSuccessNotification()
    {
        successPanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        successPanel.SetActive(false);
    }

    private void UpdateGemUI()
    {
        if (gemCounterText != null)
        {
            gemCounterText.text = "Gems: " + currentGems.ToString();
        }
    }
}