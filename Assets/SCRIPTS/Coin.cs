using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 10;

    private void OnMouseDown()
    {
        GameManager.Instance.AddRewards(coinValue, 5);
        
        Destroy(gameObject);
    }
}