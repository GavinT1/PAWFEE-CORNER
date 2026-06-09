using UnityEngine;

public class CafeTable : MonoBehaviour
{
    public int tableID;
    public bool isUnlocked = true;
    public bool isOccupied = false;
    
    [Header("Table Stats")]
    public int tableLevel = 1;
    public float baseEatTime = 3.0f;
    public int baseCoinReward = 10;
    public int baseXpReward = 5;

    void Start()
    {
        if (!isUnlocked)
        {
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentEatTime()
    {
        return Mathf.Max(1.0f, baseEatTime - (tableLevel * 0.3f));
    }

    public int GetCurrentCoinReward()
    {
        return baseCoinReward + (tableLevel * 5);
    }

    public int GetCurrentXpReward()
    {
        return baseXpReward + (tableLevel * 2);
    }
}