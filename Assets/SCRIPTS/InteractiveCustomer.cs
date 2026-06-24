using UnityEngine;
using System.Collections;

public class InteractiveCustomer : MonoBehaviour
{
    public GameObject orderBubble;
    public GameObject coinPrefab;

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
            orderBubble.SetActive(false);
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

    // ── MOVEMENT ───────────────────────────────────
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
            tableTransform.position.y + 1f,
            tableTransform.position.z
        );
    }

    // ── ORDER SEQUENCE ─────────────────────────────
    IEnumerator SettleAndShowBubble()
    {
        yield return new WaitForSeconds(1.0f);
        if (orderBubble != null)
            orderBubble.SetActive(true);
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

        // Spawn coin
        Vector3 coinPos = new Vector3(
            transform.position.x - 0.5f,
            transform.position.y,
            transform.position.z
        );
        GameObject freshCoin = Instantiate(coinPrefab, coinPos, Quaternion.identity);

        Coin coinScript = freshCoin.GetComponent<Coin>();
        if (coinScript != null)
            coinScript.coinValue = tableComponent.GetCurrentCoinReward();

        // Notify Animal Stars of successful service
        if (AnimalStarsManager.Instance != null)
            AnimalStarsManager.Instance.RecordSuccessfulService();

        // XP reward
        if (GameManager.Instance != null)
            GameManager.Instance.AddXP(tableComponent.GetCurrentXpReward());

        tableComponent.isOccupied = false;

        yield return new WaitForSeconds(0.2f);

        // Start roaming phase
        totalRoamingStops       = Random.Range(1, 4);
        currentRoamingStopCount = 0;
        isRoaming               = true;
        specificTargetPos       = GetRandomFloorSpot();
        isWalking               = true;
    }

    // ── ROAMING ────────────────────────────────────
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
}