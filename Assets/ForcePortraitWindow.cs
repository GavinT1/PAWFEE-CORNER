using UnityEngine;

public class ForcePortraitWindow : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        #if UNITY_STANDALONE && !UNITY_EDITOR
            // Enables the window borders so users can click and drag the edges to resize it freely!
            Screen.fullScreenMode = FullScreenMode.Windowed;

            // Calculates a safe dynamic height based on the player's active monitor screen height
            int calculatedHeight = Mathf.RoundToInt(Screen.currentResolution.height * 0.75f);
            
            // Keeps a perfect 3:4 aspect ratio scale matching the iPad frame dimensions
            int calculatedWidth = Mathf.RoundToInt(calculatedHeight * 0.75f);
            
            Screen.SetResolution(calculatedWidth, calculatedHeight, FullScreenMode.Windowed);
        #endif
    }
}