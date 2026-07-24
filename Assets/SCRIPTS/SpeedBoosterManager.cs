using UnityEngine;
using TMPro;

public class SpeedBoosterManager : MonoBehaviour
{
    public static SpeedBoosterManager Instance;

    [Header("UI Indicator (Optional)")]
    public GameObject speedBoosterUIBanner; // Optional active indicator banner
    public TMP_Text timerText;               // Optional text (e.g. "04:59")

    private float speedBoosterTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (speedBoosterTimer > 0)
        {
            speedBoosterTimer -= Time.deltaTime;

            if (speedBoosterUIBanner != null) speedBoosterUIBanner.SetActive(true);

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(speedBoosterTimer / 60);
                int seconds = Mathf.FloorToInt(speedBoosterTimer % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (speedBoosterTimer <= 0)
            {
                speedBoosterTimer = 0f;
                if (speedBoosterUIBanner != null) speedBoosterUIBanner.SetActive(false);
                Debug.Log("Speed Booster expired!");
            }
        }
    }

    public void ActivateSpeedBooster(float durationInSeconds = 300f) // 300s = 5 minutes
    {
        speedBoosterTimer += durationInSeconds;
        Debug.Log($"Speed Booster Activated for {durationInSeconds} seconds!");
    }

    public bool IsSpeedBoosterActive()
    {
        return speedBoosterTimer > 0;
    }
}