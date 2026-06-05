using UnityEngine;
using System.Collections;

public class CafeTable : MonoBehaviour
{
    public bool isOccupied = false;
    public float cookTime = 4.0f; // Default preparation time
    private GameObject currentCustomer;

    public void ReceiveCustomer(GameObject customer)
    {
        isOccupied = true;
        currentCustomer = customer;
        StartCoroutine(PrepareOrder());
    }

    IEnumerator PrepareOrder()
    {
        // 1. Simulates staff preparing the order
        yield return new WaitForSeconds(cookTime);

        // 2. Order complete! Earn Tier 1 rewards (10 coins, 10 XP)
        GameManager.Instance.AddRewards(10, 10); 

        // 3. Kick customer out
        Destroy(currentCustomer);
        isOccupied = false;
    }
}