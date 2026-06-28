using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI progressText;
    public RectTransform loadingCharacter;

    private static string targetSceneName = "GAMEPLAY"; 
    private float animationTimer = 0f;
    private Vector3 originalCharacterScale = Vector3.one;

    public static void LoadScene(string sceneName)
    {
        targetSceneName = sceneName;
        SceneManager.LoadScene("LOADING SCENE");
    }

    void Start()
    {
        if (loadingCharacter != null)
        {
            originalCharacterScale = loadingCharacter.localScale;
        }

        StartCoroutine(LoadSceneAsynchronously());
    }

    void Update()
    {
        if (loadingCharacter != null)
        {
            animationTimer += Time.deltaTime;

            float pulseScaleX = originalCharacterScale.x * (1f + Mathf.Sin(animationTimer * 4f) * 0.05f);
            float pulseScaleY = originalCharacterScale.y * (1f - Mathf.Sin(animationTimer * 4f) * 0.03f);
            loadingCharacter.localScale = new Vector3(pulseScaleX, pulseScaleY, originalCharacterScale.z);

            float gentleTilt = Mathf.Sin(animationTimer * 3f) * 4f;
            loadingCharacter.localRotation = Quaternion.Euler(0f, 0f, gentleTilt);
        }
    }

    IEnumerator LoadSceneAsynchronously()
    {
        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false;

        float currentProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            while (currentProgress < targetProgress)
            {
                currentProgress += Time.deltaTime * 1.2f;
                
                if (loadingBar != null) loadingBar.value = currentProgress;
                if (progressText != null) progressText.text = Mathf.RoundToInt(currentProgress * 100f) + "%";
                
                yield return null;
            }

            if (currentProgress >= 0.99f)
            {
                yield return new WaitForSeconds(0.8f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}