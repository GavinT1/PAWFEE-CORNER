using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuButtonScripts : MonoBehaviour
{
    public string gameplaySceneName = "GAMEPLAY"; 
    public GameObject settingsPanel;

    private void Start()
    {
        if (settingsPanel !=null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("PLAY button clicked! Loading scene: " + gameplaySceneName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnStoreButtonClicked()
    {
        Debug.Log("STORE button clicked!");
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
        if (settingsPanel !=null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnCloseSettingsButtonClicked()
    {
        Debug.Log("Closed Settings Button");
        if(settingsPanel !=null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnDailyRewardButtonClicked()
    {
        Debug.Log("DAILY REWARD button clicked!");
    }

    public void OnCollectOilButtonClicked()
    {
        Debug.Log("COLLECT OIL button clicked!");
    }

    public void OnCreditsButtonClicked()
    {
        Debug.Log("CREDITS button clicked!");
    }
}
