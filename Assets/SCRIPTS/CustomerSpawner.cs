using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public CafeTable[] tables; 
    public Transform spawnPoint; 

    private int promoClicksNeeded = 5;
    private int currentPromoClicks = 0;

    void Update()
    {
        CheckForOpenTables();
    }

    public void OnPromoButtonClicked()
    {
        currentPromoClicks++;

        if (currentPromoClicks >= promoClicksNeeded)
        {
            currentPromoClicks = 0; 
            SpawnCustomerIntoLine();
        }
    }

    void SpawnCustomerIntoLine()
    {
        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        InteractiveCustomer customerScript = newCustomer.GetComponent<InteractiveCustomer>();
        
        LineManager.Instance.JoinLine(customerScript);
    }

    void CheckForOpenTables()
    {
        if (!LineManager.Instance.HasCustomers()) return;

        foreach (CafeTable table in tables)
        {
            if (!table.isOccupied)
            {
                table.isOccupied = true; 

                InteractiveCustomer nextCustomer = LineManager.Instance.GetNextCustomer();
                if (nextCustomer != null)
                {
                    nextCustomer.AssignToTable(table.transform, table);
                }
                break; 
            }
        }
    }
}