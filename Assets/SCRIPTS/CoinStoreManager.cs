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
        // ── AUDIO HOOK: Play slide open sfx ─────────────────────────────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelOpen();
        }

        if (coinStorePanel != null) 
        {
            SmoothPanelAnimator animator = coinStorePanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.ShowPanel();
            }
            else
            {
                coinStorePanel.SetActive(true);
            }
        }
    }

    public void CloseCoinStore()
    {
        // ── AUDIO HOOK: Play slide close sfx ────────────────────────────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelClose();
        }

        if (coinStorePanel != null)
        {
            SmoothPanelAnimator animator = coinStorePanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.HidePanel();
            }
            else
            {
                coinStorePanel.SetActive(false);
            }
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
        if (GameManager.Instance.SpendGems(gemCost))
        {
            GameManager.Instance.AddCoins(coinReward);

            // ── AUDIO HOOK: Play the coin reward drop sfx ────────────────────
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayCoinDrop();
            }

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
            
            // OPTIONAL: Play a normal tap button click even if transaction fails 
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayButtonClick();
            }
        }
    }

    IEnumerator ShowSuccessNotification()
    {
        successPanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        successPanel.SetActive(false);
    }
}
