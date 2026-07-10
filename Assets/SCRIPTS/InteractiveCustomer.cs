using UnityEngine;
using System.Collections;

public class InteractiveCustomer : MonoBehaviour
{
    [Header("Visual Character Settings")]
    public SpriteRenderer characterSpriteRenderer; // Drag this object's own SpriteRenderer here
    public Sprite[] customerSkins; // Assign different customer skin sprites in the inspector
    public GameObject orderBubble;
    public GameObject coinPrefab;

    [Header("Bubble Position Tuning")]
    private Vector3 bubbleLocalPosition = new Vector3(-1.5f, 1.25f, 0f); // Change X to move left/right, increase Y to move higher above head!

    private Transform targetDestination;
    private CafeTable tableComponent;
    private float walkSpeed = 1.5f;
    private bool isWalking = false;
    private bool waitingInLine = true;
    private bool isLeaving = false;
    private bool isRoaming = false;
    private Vector3 specificTargetPos;
    private Vector3 spawnExitPoint;

    private int totalRoamingStops = 0;
    private int currentRoamingStopCount = 0;

    void Start()
    {
        spawnExitPoint = transform.position;
        if (orderBubble != null)
        {
            orderBubble.SetActive(false);
            orderBubble.transform.localPosition = bubbleLocalPosition;
        }

        // ── MODULAR SKIN PICKER ENGINE ────────────────────────────────────────
        // Automatically grabs the SpriteRenderer if it wasn't manually dragged in the inspector
        if (characterSpriteRenderer == null)
        {
            characterSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Picks a random character skin asset from your array of 5 unique guests
        if (customerSkins != null && customerSkins.Length > 0 && characterSpriteRenderer != null)
        {
            int randomSkinIndex = Random.Range(0, customerSkins.Length);
            if (customerSkins[randomSkinIndex] != null)
            {
                characterSpriteRenderer.sprite = customerSkins[randomSkinIndex];
            }
        }
    }

    void Update()
    {
        if (isWalking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                specificTargetPos,
                walkSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, specificTargetPos) < 0.1f)
            {
                isWalking = false;

                if (!waitingInLine && !isLeaving && !isRoaming)
                {
                    StartCoroutine(SettleAndShowBubble());
                }
                else if (isRoaming)
                {
                    StartCoroutine(PauseAndEvaluateNextMove());
                }
                else if (isLeaving)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void MoveToQueueSpot(Transform spot)
    {
        targetDestination   = spot;
        specificTargetPos   = spot.position;
        isWalking           = true;
        waitingInLine       = true;
        isLeaving           = false;
        isRoaming           = false;
    }

    public void AssignToTable(Transform tableTransform, CafeTable table)
    {
        targetDestination   = tableTransform;
        tableComponent      = table;
        waitingInLine       = false;
        isLeaving           = false;
        isRoaming           = false;
        isWalking           = true;

        specificTargetPos = new Vector3(
            tableTransform.position.x,
            tableTransform.position.y + 0.5f,
            tableTransform.position.z
        );
    }

    IEnumerator SettleAndShowBubble()
    {
       yield return new WaitForSeconds(1.0f);
        
        if (orderBubble != null)
        {
            // 1. Fetch your active recipe unlock states list array from the RecipeManager
            if (RecipeManager.Instance != null)
            {
                bool[] unlockStates = RecipeManager.Instance.GetRecipeUnlockStates();
                
                // Count how many recipes are currently unlocked/purchased
                System.Collections.Generic.List<int> unlockedIndices = new System.Collections.Generic.List<int>();
                
                for (int i = 0; i < unlockStates.Length; i++)
                {
                    if (unlockStates[i])
                    {
                        unlockedIndices.Add(i); // Add valid index choice (e.g., 0 for Coffee, 1 for Tea)
                    }
                }

                // 2. Select a random index explicitly from your unlocked list choices array
                if (unlockedIndices.Count > 0)
                {
                    int randomIndexChooser = Random.Range(0, unlockedIndices.Count);
                    int chosenRecipeIndex = unlockedIndices[randomIndexChooser];

                    // 3. Pass that specific chosen food index number to the order bubble script
                    OrderBubble bubbleScript = orderBubble.GetComponent<OrderBubble>();
                    if (bubbleScript != null)
                    {
                        bubbleScript.SetOrderGraphic(chosenRecipeIndex);
                    }
                }
            }

            // Fix: Ensures the bubble updates its location right when it becomes visible
            orderBubble.transform.localPosition = bubbleLocalPosition;
            orderBubble.SetActive(true);
        }
    }

    public void OnBubbleClicked()
    {
        if (orderBubble != null && orderBubble.activeSelf)
        {
            orderBubble.SetActive(false);
            StartCoroutine(CookingAndEatingSequence());
        }
    }

    IEnumerator CookingAndEatingSequence()
    {
        yield return new WaitForSeconds(2.0f);

        float dynamicEatTime = tableComponent.GetCurrentEatTime();
        yield return new WaitForSeconds(dynamicEatTime);

        Vector3 coinPos = new Vector3(
            transform.position.x - 0.5f,
            transform.position.y,
            transform.position.z
        );
        GameObject freshCoin = Instantiate(coinPrefab, coinPos, Quaternion.identity);

        Coin coinScript = freshCoin.GetComponent<Coin>();
        if (coinScript != null)
        {
            coinScript.coinValue = tableComponent.GetCurrentCoinReward();
            coinScript.TriggerPhysicalPopAnimation();
        }

        if (AnimalStarsManager.Instance != null)
            AnimalStarsManager.Instance.RecordSuccessfulService();

        if (GameManager.Instance != null)
            GameManager.Instance.AddXP(tableComponent.GetCurrentXpReward());

        tableComponent.isOccupied = false;

        yield return new WaitForSeconds(0.2f);

        totalRoamingStops       = Random.Range(1, 4);
        currentRoamingStopCount = 0;
        isRoaming               = true;
        specificTargetPos       = GetRandomFloorSpot();
        isWalking               = true;
    }

    IEnumerator PauseAndEvaluateNextMove()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

        currentRoamingStopCount++;

        if (currentRoamingStopCount < totalRoamingStops)
        {
            specificTargetPos = GetRandomFloorSpot();
            isWalking = true;
        }
        else
        {
            isRoaming         = false;
            isLeaving         = true;
            specificTargetPos = spawnExitPoint;
            isWalking         = true;
        }
    }

    Vector3 GetRandomFloorSpot()
    {
        float randomX = Random.Range(-2.5f, 2.5f);
        float randomY = Random.Range(-2.0f, 1.5f);
        return new Vector3(randomX, randomY, transform.position.z);
    }

    void OnEnable()
    {
        if (spawnExitPoint == Vector3.zero) return;

        StopAllCoroutines();

        if (isWalking) return;

        if (isRoaming)
        {
            StartCoroutine(PauseAndEvaluateNextMove());
        }
        else if (isLeaving)
        {
            isWalking = true;
        }
        else if (!waitingInLine && tableComponent != null && !tableComponent.isOccupied)
        {
            totalRoamingStops = Random.Range(1, 4);
            currentRoamingStopCount = 0;
            isRoaming = true;
            specificTargetPos = GetRandomFloorSpot();
            isWalking = true;
        }
        else if (!waitingInLine && orderBubble != null && !orderBubble.activeSelf)
        {
            if (tableComponent != null && tableComponent.isOccupied)
            {
                StartCoroutine(CookingAndEatingSequence());
            }
        }
        else if (!waitingInLine && orderBubble != null && orderBubble.activeSelf)
        {
            orderBubble.SetActive(true);
        }
    }
}