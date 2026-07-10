using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    [Header("Sprite Graphic Component Reference")]
    public SpriteRenderer spriteRenderer; 
    [Header("Pre-baked Order Bubble Sprites (Assign all 5 in order)")]
    [Tooltip("Element 0: Coffee Bubble, Element 1: Tea Bubble, Element 2: Muffin Bubble, etc.")]
    public Sprite[] orderBubbleSprites;

    private InteractiveCustomer parentCustomer;

    void Awake()
    {
        // Cache the reference to the parent customer script component
        parentCustomer = GetComponentInParent<InteractiveCustomer>();
        
        // Auto-grab the SpriteRenderer if it wasn't manually dragged in the inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // ── CALLED BY CUSTOMER TO SWAP SPRIRE ARTWORK DYNAMICALLY ────────────────
    public void SetOrderGraphic(int recipeIndex)
    {
        if (orderBubbleSprites == null || orderBubbleSprites.Length == 0) return;

        // Safety array bounds check to prevent crash errors
        if (recipeIndex >= 0 && recipeIndex < orderBubbleSprites.Length)
        {
            if (spriteRenderer != null && orderBubbleSprites[recipeIndex] != null)
            {
                // Swaps the visual texture graphic instantly on the screen!
                spriteRenderer.sprite = orderBubbleSprites[recipeIndex];
            }
        }
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
