using UnityEngine;


public class CustomerSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject customerPrefab;
    public CafeTable[] tables; 
    public Transform spawnPoint; 

    [Header("Auto Trickle Settings")]
    public float autoSpawnInterval = 8f;
    private float autoSpawnTimer = 0f;

    [Header("Promo Button Settings")]
    public int promoClicksNeeded = 5;
    public float promoCooldownDuration = 15f;
    private bool promoOnCooldown = false;
    private float promoCooldownTimer = 0f;
    private int currentPromoClicks = 0;

    void Update()
    {   
        HandleAutoSpawn();
        HandlePromoCooldown();
        CheckForOpenTables();
    }

// -- AUTO TRICKLE SPAWN ------------------------------------------------------------------------
    void HandleAutoSpawn()
    {
        float speedMultiplier = 1.0f;

        if(AnimalStarsManager.Instance != null)
        {
            speedMultiplier = AnimalStarsManager.Instance.GetSpawnRateMultiplier();
        }

        autoSpawnTimer += Time.deltaTime * speedMultiplier;

        if (autoSpawnTimer >= autoSpawnInterval)
        {
            autoSpawnTimer = 0f;
            SpawnCustomerIntoLine();
        }
    }

// -- PROMO BUTTON COOLDOWN ------------------------------------------------------------------------
    public void OnPromoButtonClicked()
    {
        if (promoOnCooldown)
        {
            Debug.Log("Promo button is on cooldown!");
            return;
        }

        currentPromoClicks++;

        if (currentPromoClicks >= promoClicksNeeded)
        {
            currentPromoClicks = 0;

            // Spawn Bursts 3 customers
            for (int i = 0; i < 3; i++)
            {
                SpawnCustomerIntoLine();
            }

            // Start cooldown
            promoOnCooldown = true;
            promoCooldownTimer = promoCooldownDuration;
        }
    }

    void HandlePromoCooldown()
    {
        if (!promoOnCooldown) return;

        promoCooldownTimer -= Time.deltaTime;

        if (promoCooldownTimer <= 0f)
        {
            promoOnCooldown = false;
            promoCooldownTimer = 0f;
            currentPromoClicks = 0;
        }
    }

// -- SPAWN LOGIC ------------------------------------------------------------------------
    void SpawnCustomerIntoLine()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point not assigned in CustomerSpawner!");
            return;
        }
        if (customerPrefab == null)
        {
            Debug.LogError("Customer prefab not assigned in CustomerSpawner!");
            return;
        }
        
        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        InteractiveCustomer customerScript = newCustomer.GetComponent<InteractiveCustomer>();
        
        if (customerScript != null)
        {
            LineManager.Instance.JoinLine(customerScript);

            // Trigger a flying danmaku comment when a customer joins the line!
            if (DanmakuManager.Instance != null)
            {
                DanmakuManager.Instance.SpawnCustomerComment();
            }
        }
        else
        {
            Debug.LogError("Spawned customer does not have an InteractiveCustomer script attached!");
        }
    }

// -- TABLE ASSIGNMENT LOGIC ------------------------------------------------------------------------
    void CheckForOpenTables()
    {
        if (!LineManager.Instance.HasCustomers()) return;

        foreach (CafeTable table in tables)
        {
            
            if (table.isUnlocked && !table.isOccupied)
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