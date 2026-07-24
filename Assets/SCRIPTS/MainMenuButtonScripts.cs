using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScripts : MonoBehaviour
{
    public string gameplaySceneName = "LOADING SCENE"; 
    public GameObject settingsPanel;
    public GameObject shopPanel;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("PLAY button clicked! Loading scene: " + gameplaySceneName);
        
        // ── AUDIO HOOK: Play crisp click on menu selection
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();

        LoadingScreenManager.LoadScene(gameplaySceneName);
    }

    public void OnStoreButtonClicked()
    {
        Debug.Log("STORE button clicked!");
        
        // ── AUDIO HOOK: Play slide open sfx
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelOpen();

        if (shopPanel != null)
        {
            SmoothPanelAnimator animator = shopPanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.ShowPanel();
            }
            else
            {
                shopPanel.SetActive(true);
            }
        }
    }

    public void OnCloseStoreButtonClicked()
    {
        Debug.Log("Closed Store Button");
        
        // ── AUDIO HOOK: Play slide close sfx
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelClose();

        if (shopPanel != null)
        {
            SmoothPanelAnimator animator = shopPanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.HidePanel();
            }
            else
            {
                shopPanel.SetActive(false);
            }
        }
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
        
        // ── AUDIO HOOK: Play slide open sfx
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelOpen();

        if (settingsPanel != null)
        {
            SmoothPanelAnimator animator = settingsPanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.ShowPanel();
            }
            else
            {
                settingsPanel.SetActive(true);
            }
        }
    }

    public void OnCloseSettingsButtonClicked()
    {
        Debug.Log("Closed Settings Button");
        
        // ── AUDIO HOOK: Play slide close sfx
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPanelClose();

        if (settingsPanel != null)
        {
            SmoothPanelAnimator animator = settingsPanel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.HidePanel();
            }
            else
            {
                settingsPanel.SetActive(false);
            }
        }
    }

    public void OnDailyRewardButtonClicked()
    {
        Debug.Log("DAILY REWARD button clicked!");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
    }

    public void OnCollectOilButtonClicked()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        if (DailyRewardManager.Instance != null) DailyRewardManager.Instance.OpenDailyReward();
    }

    public void OnCreditsButtonClicked()
    {
        Debug.Log("CREDITS button clicked!");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
    }
}
