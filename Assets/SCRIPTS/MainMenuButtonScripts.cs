using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScripts : MonoBehaviour
{
    public string gameplaySceneName = "LOADING SCENE"; 
    public GameObject settingsPanel;
    public GemStoreManager gemStoreManager;
    public CoinStoreManager coinStoreManager;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("PLAY button clicked! Loading scene: " + gameplaySceneName);
        LoadingScreenManager.LoadScene(gameplaySceneName);
    }

    public void OnStoreButtonClicked()
    {
        Debug.Log("STORE button clicked!");
        if (gemStoreManager != null)
        {
            gemStoreManager.OpenGemStore();
        }
    }

    public void OnCoinStoreButtonClicked()
    {
        Debug.Log("COIN STORE button clicked!");
        if (coinStoreManager != null)
        {
            coinStoreManager.OpenCoinStore();
        }
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
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
    }

    public void OnCollectOilButtonClicked()
    {
        if (DailyRewardManager.Instance != null) DailyRewardManager.Instance.OpenDailyReward();
    }

    public void OnCreditsButtonClicked()
    {
        Debug.Log("CREDITS button clicked!");
    }
}