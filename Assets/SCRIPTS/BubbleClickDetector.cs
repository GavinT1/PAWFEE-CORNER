using UnityEngine;

public class BubbleClickDetector : MonoBehaviour
{
    private InteractiveCustomer parentCustomer;

    void Start()
    {
        parentCustomer = GetComponentInParent<InteractiveCustomer>();
    }

    void OnMouseDown()
    {
        if (parentCustomer != null)
        {
            parentCustomer.OnBubbleClicked();
        }
    }
}