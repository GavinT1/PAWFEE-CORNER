using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonToggle : MonoBehaviour
{
    [Header("UI Raw Images")]
    [SerializeField] private RawImage onRawImage;
    [SerializeField] private RawImage offRawImage;

    [Header("Initial State")]
    [SerializeField] private bool isOn = false;

    private Button toggleButton;

    private void Awake()
    {
        toggleButton = GetComponent<Button>();
        toggleButton.onClick.AddListener(OnButtonClicked);
        UpdateVisuals();
    }

    private void OnButtonClicked()
    {
        isOn = !isOn;
        UpdateVisuals();
        OnToggleChanged(isOn);
    }

    private void UpdateVisuals()
    {
        if (onRawImage != null && offRawImage != null)
        {
            onRawImage.gameObject.SetActive(isOn);
            offRawImage.gameObject.SetActive(!isOn);
        }
    }

    private void OnToggleChanged(bool state)
    {
        Debug.Log("Switch is now: " + (state ? "ON" : "OFF"));
    }
    // code here usage of ButtonToggle class 
}