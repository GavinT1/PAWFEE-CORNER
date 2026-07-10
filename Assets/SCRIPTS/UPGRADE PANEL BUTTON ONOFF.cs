using UnityEngine;

public class UPGRADEPANELBUTTONONOFF : MonoBehaviour
{
    public GameObject panel;

    public void OpenPanel()
    {
        if (panel != null)
        {
            SmoothPanelAnimator animator = panel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.ShowPanel();
            }
            else
            {
                panel.SetActive(true);
            }
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            SmoothPanelAnimator animator = panel.GetComponent<SmoothPanelAnimator>();
            if (animator != null)
            {
                animator.HidePanel();
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }
}
