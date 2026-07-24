using UnityEngine;
using System.Collections;

public class GemStoreManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gemStorePanel;       
    public GameObject successPanel;        

    void Start()
    {
        if (gemStorePanel != null) gemStorePanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    public void OpenGemStore()
    {
        // ── AUDIO HOOK: Play slide open sfx ─────────────────────────────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelOpen();
        }

        if (gemStorePanel != null)
        {
            SmoothPanelAnimator animator = gemStorePanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.ShowPanel();
            }
            else
            {
                gemStorePanel.SetActive(true);
            }
        }
    }

    public void CloseGemStore()
    {
        // ── AUDIO HOOK: Play slide close sfx ────────────────────────────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPanelClose();
        }

        if (gemStorePanel != null) 
        {
            SmoothPanelAnimator animator = gemStorePanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.HidePanel();
            }
            else
            {
                gemStorePanel.SetActive(false);
            }
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

    private void ProcessFakePurchase(int gemAmount)
    {
        GameManager.Instance.AddGems(gemAmount);

        // ── AUDIO HOOK: Play crisp purchase click sfx ───────────────────────
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
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

    IEnumerator ShowSuccessNotification()
    {
        successPanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        successPanel.SetActive(false);
    }
}
