using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public CafeTable[] tables; 
    public Transform spawnPoint; 
    public float spawnInterval = 4.0f;

    void Start()
    {
        InvokeRepeating("TrySpawnCustomer", 1.0f, spawnInterval);
    }

    void TrySpawnCustomer()
    {
        foreach (CafeTable table in tables)
        {
            if (!table.isOccupied)
            {
                table.isOccupied = true; 

                
                GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                
                
                InteractiveCustomer customerScript = newCustomer.GetComponent<InteractiveCustomer>();
                customerScript.SetupCustomer(table.transform, table);
                
                break; 
            }
        }
    }
}