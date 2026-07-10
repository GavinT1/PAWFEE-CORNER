using UnityEngine;
using System.IO; 

public class DevCheatMenu : MonoBehaviour
{
    public void ResetGameSaveData()
    {
        Debug.LogWarning("DevCheatMenu: Wiping save file from disk and restarting fresh!");

        string savePath = Path.Combine(Application.persistentDataPath, "pawfeesave.json");

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"Deleted file at: {savePath}");
        }

        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel = 1; 
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void CheatLevelUp()
    {
        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel++;
            
            Debug.Log($"DevCheatMenu: Cheated Level Up! Current Level is now: {LevelSystem.Instance.currentLevel}");

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnPlayerLevelUp();
            }

            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.Save();
            }
        }
        else
        {
            Debug.LogError("DevCheatMenu: Could not find LevelSystem.Instance in your scene!");
        }
    }
}