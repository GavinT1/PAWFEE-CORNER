using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DanmakuManager : MonoBehaviour
{
    public static DanmakuManager Instance;

    [Header("Prefab Setup")]
    public GameObject danmakuPrefab;
    public RectTransform spawnContainer;

    [Header("Movement Settings")]
    public float moveSpeed = 150f;
    
    [Header("Track Configuration")]
    public int totalTracks = 5;
    public float trackHeight = 70f;
    public float topPadding = 180f;

    private string[] customerComments = new string[]
    {
        "Welcome to Pawfee Corner!"
    };

    private int lastTrackIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SpawnCustomerComment()
    {
        if (danmakuPrefab == null || spawnContainer == null) return;

        string randomComment = customerComments[Random.Range(0, customerComments.Length)];
        
        GameObject newTextObj = Instantiate(danmakuPrefab, spawnContainer);
        RectTransform rect = newTextObj.GetComponent<RectTransform>();
        TextMeshProUGUI textComp = newTextObj.GetComponentInChildren<TextMeshProUGUI>();

        if (textComp != null)
        {
            textComp.text = randomComment;
        }

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        int currentTrack = Random.Range(0, totalTracks);
        if (currentTrack == lastTrackIndex)
        {
            currentTrack = (currentTrack + 1) % totalTracks;
        }
        lastTrackIndex = currentTrack;

        float calculatedY = -topPadding - (currentTrack * trackHeight);
        
        float randomXOffset = Random.Range(0f, 180f);
        float startX = spawnContainer.rect.width + 50f + randomXOffset; 
        
        rect.anchoredPosition = new Vector2(startX, calculatedY);

        Canvas.ForceUpdateCanvases();
        
        HorizontalLayoutGroup layout = newTextObj.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            layout.enabled = false;
            layout.enabled = true;
        }

        float totalWidth = rect.rect.width > 0 ? rect.rect.width : 500f;

        StartCoroutine(MoveCommentRoutine(rect, totalWidth));
    }

    private IEnumerator MoveCommentRoutine(RectTransform rect, float totalWidth)
    {
        float targetX = -totalWidth - 100f;

        while (rect != null && rect.anchoredPosition.x > targetX)
        {
            rect.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;
            yield return null;
        }

        if (rect != null)
        {
            Destroy(rect.gameObject);
        }
    }
}