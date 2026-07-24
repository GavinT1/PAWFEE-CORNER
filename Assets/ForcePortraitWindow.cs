using UnityEngine;

public class ForcePortraitWindow : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        #if UNITY_STANDALONE && !UNITY_EDITOR
            // Enables windowed mode so the user can see the full mobile frame
            Screen.fullScreenMode = FullScreenMode.Windowed;

            // Calculates height dynamically based on monitor height
            int calculatedHeight = Mathf.RoundToInt(Screen.currentResolution.height * 0.8f);
            
            // Forces 9:16 portrait ratio (1080x1920 ratio = 9/16 = 0.5625)
            int calculatedWidth = Mathf.RoundToInt(calculatedHeight * (9f / 16f));
            
            Screen.SetResolution(calculatedWidth, calculatedHeight, FullScreenMode.Windowed);
        #endif
    }
}