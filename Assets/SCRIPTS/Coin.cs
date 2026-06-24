using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 10;
    private const int BaseXpReward = 5;

    // ── INTERACTION ────────────────────────────────    
    private void OnMouseDown()
    {
        int finalPayout = coinValue;

        //adjust value based on active star rating

        if (AnimalStarsManager.Instance != null)
        {
            float starMultiplier = AnimalStarsManager.Instance.GetCoinMultiplier();
            finalPayout = Mathf.RoundToInt(coinValue * starMultiplier);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRewards(finalPayout, BaseXpReward);
        }
        
        Destroy(gameObject);
    }
}