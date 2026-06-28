using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class PromoButtonJuicer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Coroutine punchCoroutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }
        punchCoroutine = StartCoroutine(AnimatePressDown());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }
        punchCoroutine = StartCoroutine(AnimateReleaseBounce());
    }

    IEnumerator AnimatePressDown()
    {
        float time = 0f;
        float duration = 0.05f;
        Vector3 startScale = transform.localScale;
        Vector3 targetSquash = new Vector3(originalScale.x * 1.12f, originalScale.y * 0.88f, originalScale.z);

        while (time < duration)
        {
            time += Time.deltaTime;
            float ratio = Mathf.Clamp01(time / duration);
            transform.localScale = Vector3.Lerp(startScale, targetSquash, ratio);
            yield return null;
        }
        transform.localScale = targetSquash;
    }

    IEnumerator AnimateReleaseBounce()
    {
        float time = 0f;
        float duration1 = 0.05f;
        Vector3 startScale = transform.localScale;
        Vector3 overshootScale = new Vector3(originalScale.x * 0.92f, originalScale.y * 1.08f, originalScale.z);

        while (time < duration1)
        {
            time += Time.deltaTime;
            float ratio = Mathf.Clamp01(time / duration1);
            transform.localScale = Vector3.Lerp(startScale, overshootScale, ratio);
            yield return null;
        }

        time = 0f;
        float duration2 = 0.08f;
        while (time < duration2)
        {
            time += Time.deltaTime;
            float ratio = Mathf.Clamp01(time / duration2);
            transform.localScale = Vector3.Lerp(overshootScale, originalScale, ratio);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }
        transform.localScale = originalScale;
    }
}