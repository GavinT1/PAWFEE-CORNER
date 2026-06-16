using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Tables Reference")]
    public CafeTable[] tables; 

    [Header("UI Buttons Text Display")]
    public TMP_Text[] buttonTexts; 

    private int[] upgradeCosts = new int[] { 50, 60, 70, 80, 90 };
    private int[] unlockCosts = new int[] { 0, 150, 300, 500, 800 }; 

    void Start()
    {
        UpdateUpgradeUI();
    }

    public void OnClickUpgradeRow(int index)
    {
        if (index < 0 || index >= tables.Length) return;

        CafeTable targetTable = tables[index];

        if (!targetTable.isUnlocked)
        {
            if (GameManager.Instance.SpendCoins(unlockCosts[index]))
            {
                targetTable.isUnlocked = true;
                targetTable.gameObject.SetActive(true);
                UpdateUpgradeUI();
            }
        }
        else
        {
            if (GameManager.Instance.SpendCoins(upgradeCosts[index]))
            {
                targetTable.tableLevel++;
                upgradeCosts[index] = Mathf.RoundToInt(upgradeCosts[index] * 1.5f);
                UpdateUpgradeUI();
            }
        }
    }

    void UpdateUpgradeUI()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            if (i >= buttonTexts.Length) break;

            if (!tables[i].isUnlocked)
            {
                buttonTexts[i].text = "Buy Table " + (i + 1) + ": " + unlockCosts[i] + " Coins";
            }
            else
            {
                buttonTexts[i].text = "Upgrade T" + (i + 1) + ": " + upgradeCosts[i] + " Coins";
            }
        }
    }
}