using UnityEngine;
using System.Collections;

public class ThiefCustomer : MonoBehaviour
{
    [Header("Thief Settings")]
    public int coinsToSteal = 500;       
    public int totalTapsNeeded = 5;      
    public float walkSpeed = 0.6f;
    
    private int currentTaps = 0;
    private bool hasStolen = false;
    private Vector3 specificTargetPos;
    private bool isWalking = false;

    void Start()
    {
        if (transform.parent == null || transform.parent.name != "MainAreaGroup")
        {
            GameObject mainGroup = GameObject.Find("MainAreaGroup");
            if (mainGroup != null)
            {
                transform.SetParent(mainGroup.transform, true);
            }
        }

        StealMoney();

        specificTargetPos = GetRandomFloorSpot();
        isWalking = true;
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
                StartCoroutine(PauseBeforeNextSpot());
            }
        }
    }

    IEnumerator PauseBeforeNextSpot()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        
        specificTargetPos = GetRandomFloorSpot();
        isWalking = true;
    }

    Vector3 GetRandomFloorSpot()
    {
        float randomX = Random.Range(-2.3f, 2.3f);
        float randomY = Random.Range(-1.8f, 1.3f);
        return new Vector3(randomX, randomY, transform.position.z);
    }

    void StealMoney()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRewards(-coinsToSteal, 0);
            hasStolen = true;
            Debug.Log($"A thief stole {coinsToSteal} coins! Tap them quick!");
        }
        
        SpriteRenderer spriteRen = GetComponent<SpriteRenderer>();
        if (spriteRen != null)
        {
            spriteRen.color = new Color(0.8f, 0.3f, 0.3f, 1f);
        }
    }

    private void OnMouseDown()
    {
        currentTaps++;
        Debug.Log($"Thief Tapped! Count: {currentTaps}/{totalTapsNeeded}");

        transform.localScale = Vector3.one * 1.15f;
        Invoke("ResetScale", 0.05f);

        if (currentTaps >= totalTapsNeeded)
        {
            GetChasedAway();
        }
    }

    void ResetScale()
    {
        transform.localScale = Vector3.one;
    }

    void GetChasedAway()
    {
        Debug.Log("The thief was defeated and dropped the stolen tips!");
        
        if (GameManager.Instance != null && hasStolen)
        {
            GameManager.Instance.AddRewards(coinsToSteal, 0);
        }

        Destroy(gameObject);
    }
}