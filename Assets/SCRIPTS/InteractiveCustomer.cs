using UnityEngine;
using System.Collections;

public class InteractiveCustomer : MonoBehaviour
{
    public GameObject orderBubble; 
    public GameObject coinPrefab;   
    
    private Transform targetDestination;
    private CafeTable tableComponent;
    private float walkSpeed = 4f;
    private bool isWalking = false;
    private bool waitingInLine = true;
    private Vector3 specificTargetPos;

    void Update()
    {
        if (isWalking)
        {
            transform.position = Vector3.MoveTowards(transform.position, specificTargetPos, walkSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, specificTargetPos) < 0.1f)
            {
                isWalking = false;
                
                if (!waitingInLine)
                {
                    StartCoroutine(SettleAndShowBubble());
                }
            }
        }
    }

    public void MoveToQueueSpot(Transform spot)
    {
        targetDestination = spot;
        specificTargetPos = spot.position;
        isWalking = true;
        waitingInLine = true;
    }

    public void AssignToTable(Transform tableTransform, CafeTable table)
    {
        targetDestination = tableTransform;
        tableComponent = table;
        waitingInLine = false;
        isWalking = true;

        specificTargetPos = new Vector3(tableTransform.position.x + 1.5f, tableTransform.position.y, tableTransform.position.z);
    }

    IEnumerator SettleAndShowBubble()
    {
        yield return new WaitForSeconds(1.0f); 
        orderBubble.SetActive(true);
    }

    public void OnBubbleClicked()
    {
        orderBubble.SetActive(false); 
        StartCoroutine(CookingAndEatingSequence());
    }

    IEnumerator CookingAndEatingSequence()
    {
        yield return new WaitForSeconds(2.0f); 
        yield return new WaitForSeconds(3.0f); 

        Vector3 floorPos = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        Instantiate(coinPrefab, floorPos, Quaternion.identity);

        tableComponent.isOccupied = false;
        Destroy(gameObject);
    }
}