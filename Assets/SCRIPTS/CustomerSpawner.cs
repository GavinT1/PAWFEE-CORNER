using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public CafeTable[] tables; // Drag your scene tables here in the inspector
    public float spawnInterval = 5.0f;

    void Start()
    {
        InvokeRepeating("TrySpawnCustomer", 2.0f, spawnInterval);
    }

   void TrySpawnCustomer()
{
    foreach (CafeTable table in tables)
    {
        if (!table.isOccupied)
        {
            Vector3 tablePos = table.transform.position;

            Vector3 customerPos = new Vector3(tablePos.x + 1.5f, tablePos.y, tablePos.z);

            GameObject newCustomer = Instantiate(customerPrefab, customerPos, Quaternion.identity);

            table.ReceiveCustomer(newCustomer);

        }
    }
}

}