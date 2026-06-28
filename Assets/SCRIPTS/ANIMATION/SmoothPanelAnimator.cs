using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SmoothPanelAnimator : MonoBehaviour
{
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateOpen());
    }

    public void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateOpen()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = originalScale * 0.4f;
        float time = 0f;
        float duration = 0.35f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            
            // Evaluates elastic stretch curve
            float curveValue = openCurve.Evaluate(progress);
            
            transform.localScale = Vector3.LerpUnclamped(originalScale * 0.4f, originalScale, curveValue);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress * 2f);
            yield return null;
        }

        transform.localScale = originalScale;
        canvasGroup.alpha = 1f;
    }

    IEnumerator AnimateClose()
    {
        float time = 0f;
        float duration = 0.2f;
        Vector3 startScale = transform.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;

            transform.localScale = Vector3.Lerp(startScale, originalScale * 0.6f, progress);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}