using UnityEngine;
using TMPro;

public class CurrencyUIDisplay : MonoBehaviour
{
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI coinText;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null) return;

        if (gemText != null) gemText.text = GameManager.Instance.coins.ToString();
        if (coinText != null) coinText.text = GameManager.Instance.gems.ToString();
    }
}