using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    private InteractiveCustomer parentCustomer;

    void Start()
    {
        parentCustomer = GetComponentInParent<InteractiveCustomer>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Bubble was physically clicked!");
        if (parentCustomer != null)
        {
            parentCustomer.OnBubbleClicked();
        }
    }
}