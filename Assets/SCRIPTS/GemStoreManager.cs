using UnityEngine;
using System.Collections;

public class GemStoreManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gemStorePanel;       
    public GameObject successPanel;        

    void Start()
    {
        if (gemStorePanel != null)
        {
            gemStorePanel.SetActive(false);
        }
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
    }

// -- OPEN/CLOSE ------------------------------------------------------------------------
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

//-- BUNDLE PURCHASES ------------------------------------------------------------------------

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

//--- CORE LOGIC ------------------------------------------------------------------------
    private void ProcessFakePurchase(int gemAmount)
    {
        GameManager.Instance.AddGems(gemAmount);

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
}