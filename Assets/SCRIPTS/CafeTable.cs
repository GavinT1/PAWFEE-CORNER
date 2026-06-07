using UnityEngine;
using System.Collections;

public class CafeTable : MonoBehaviour
{
    public bool isOccupied = false;
    public float cookTime = 4.0f; 
    private GameObject currentCustomer;

    public void ReceiveCustomer(GameObject customer)
    {
        isOccupied = true;
        currentCustomer = customer;
        StartCoroutine(PrepareOrder());
    }

    IEnumerator PrepareOrder()
    {
        
        yield return new WaitForSeconds(cookTime);

        
        GameManager.Instance.AddRewards(10, 10); 

        
        Destroy(currentCustomer);
        isOccupied = false;
    }
}