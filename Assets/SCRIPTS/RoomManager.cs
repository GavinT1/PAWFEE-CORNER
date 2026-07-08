using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Area Layout Groups")]
    public GameObject mainAreaGroup;
    public GameObject kitchenAreaGroup;

    public void SwitchToKitchen()
    {
        if (mainAreaGroup != null) mainAreaGroup.SetActive(false);
        if (kitchenAreaGroup != null) kitchenAreaGroup.SetActive(true);
        
        Debug.Log("Switched view to Kitchen!");
    }

    public void SwitchToMainArea()
    {
        if (mainAreaGroup != null) mainAreaGroup.SetActive(true);
        if (kitchenAreaGroup != null) kitchenAreaGroup.SetActive(false);
        
        Debug.Log("Switched view back to Main Area!");
    }
}