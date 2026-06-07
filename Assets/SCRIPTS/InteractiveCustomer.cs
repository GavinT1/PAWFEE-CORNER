using UnityEngine;
using System.Collections;

public class InteractiveCustomer : MonoBehaviour
{
    public GameObject orderBubble; 
    public GameObject coinPrefab;   
    
    private Transform targetTable;
    private CafeTable tableComponent;
    private float walkSpeed = 3f;
    private bool isWalking = false;

    public void SetupCustomer(Transform tableTransform, CafeTable table)
    {
        targetTable = tableTransform;
        tableComponent = table;
        orderBubble.SetActive(false); 
        isWalking = true;
    }

    void Update()
    {
        if (isWalking && targetTable != null)
        {
            Vector3 targetPos = new Vector3(targetTable.position.x + 1.2f, targetTable.position.y, targetTable.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isWalking = false;
                ArrivedAtTable();
            }
        }
    }

    void ArrivedAtTable()
    {
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