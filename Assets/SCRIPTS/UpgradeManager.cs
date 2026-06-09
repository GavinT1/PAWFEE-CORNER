using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public CafeTable table1;
    public CafeTable table2;

    [Header("UI Text Display")]
    public TMP_Text table1ButtonText;
    public TMP_Text table2ButtonText;

    private int table1UpgradeCost = 50;
    private int table2UnlockCost = 150;
    private int table2UpgradeCost = 60;

    void Start()
    {
        UpdateUpgradeUI();
    }

    public void OnClickRow1()
    {
        if (GameManager.Instance.SpendCoins(table1UpgradeCost))
        {
            table1.tableLevel++;
            table1UpgradeCost = Mathf.RoundToInt(table1UpgradeCost * 1.5f);
            UpdateUpgradeUI();
        }
    }

    public void OnClickRow2()
    {
        if (!table2.isUnlocked)
        {
            if (GameManager.Instance.SpendCoins(table2UnlockCost))
            {
                table2.isUnlocked = true;
                table2.gameObject.SetActive(true);
                UpdateUpgradeUI();
            }
        }
        else
        {
            if (GameManager.Instance.SpendCoins(table2UpgradeCost))
            {
                table2.tableLevel++;
                table2UpgradeCost = Mathf.RoundToInt(table2UpgradeCost * 1.5f);
                UpdateUpgradeUI();
            }
        }
    }

    void UpdateUpgradeUI()
    {
        table1ButtonText.text = "Upgrade T1: " + table1UpgradeCost + " Coins";

        if (!table2.isUnlocked)
        {
            table2ButtonText.text = "Buy Table 2: " + table2UnlockCost + " Coins";
        }
        else
        {
            table2ButtonText.text = "Upgrade T2: " + table2UpgradeCost + " Coins";
        }
    }
}