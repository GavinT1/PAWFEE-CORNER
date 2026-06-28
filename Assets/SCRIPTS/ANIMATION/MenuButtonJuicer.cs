using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class MenuButtonJuicer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAnimation();
        scaleCoroutine = StartCoroutine(ScaleTo(originalScale * 0.85f, 0.1f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAnimation();
        scaleCoroutine = StartCoroutine(BounceBack());
    }

    private void StopAnimation()
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float time = 0;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = target;
    }

    IEnumerator BounceBack()
    {
        float time = 0;
        float duration = 0.3f;
        Vector3 punchScale = originalScale * 1.1f;

        // Bounce Up
        while (time < 0.1f)
        {
            transform.localScale = Vector3.Lerp(originalScale * 0.85f, punchScale, time / 0.1f);
            time += Time.deltaTime;
            yield return null;
        }
        
        // Settle Back down down
        time = 0;
        while (time < 0.2f)
        {
            transform.localScale = Vector3.Lerp(punchScale, originalScale, time / 0.2f);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}