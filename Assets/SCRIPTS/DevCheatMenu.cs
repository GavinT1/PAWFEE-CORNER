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
        if (SoundManager.Instance != null) SoundManager.Instance.PlayLevelUp();

        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel++;
            
            Debug.Log($"DevCheatMenu: Cheated Level Up! Current Level is now: {LevelSystem.Instance.currentLevel}");

            // ── FIXED: Added explicit UI text and slider redraw calls ─────────
            LevelSystem.Instance.UpdateLevelUI();

            // Refresh table shop button prices/level-gate displays
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnPlayerLevelUp();
            }

            // Refresh recipe shop prices/level-gate displays if menu is open
            if (RecipeManager.Instance != null)
            {
                RecipeManager.Instance.RefreshRecipeUI();
                RecipeManager.Instance.SelectRecipeCard(0); // Safely forces the top preview to refresh
            }

            // Save the progress instantly to disk
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