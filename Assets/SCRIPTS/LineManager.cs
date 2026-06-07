using UnityEngine;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance;
    
    public List<Transform> queuePositions; 
    private List<InteractiveCustomer> customersInLine = new List<InteractiveCustomer>();

    private void Awake()
    {
        Instance = this;
    }

    public void JoinLine(InteractiveCustomer customer)
    {
        customersInLine.Add(customer);
        UpdateCustomerPositions();
    }

    public bool HasCustomers()
    {
        return customersInLine.Count > 0;
    }

    public InteractiveCustomer GetNextCustomer()
    {
        if (customersInLine.Count > 0)
        {
            InteractiveCustomer nextCustomer = customersInLine[0];
            customersInLine.RemoveAt(0);
            UpdateCustomerPositions();
            return nextCustomer;
        }
        return null;
    }

    private void UpdateCustomerPositions()
    {
        for (int i = 0; i < customersInLine.Count; i++)
        {
            if (i < queuePositions.Count)
            {
                customersInLine[i].MoveToQueueSpot(queuePositions[i]);
            }
        }
    }
}