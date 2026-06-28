using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 10;
    private const int BaseXpReward = 5;

    private Vector3 baselineOrigin;
    private Vector3 targetDropFloor;
    private float animationAge = 0f;
    private bool canCollect = false;

    public void TriggerPhysicalPopAnimation()
    {
        baselineOrigin = transform.position;
        
        float randomX = Random.Range(-1.2f, 1.2f);
        float randomY = Random.Range(-1.0f, -0.5f);
        targetDropFloor = baselineOrigin + new Vector3(randomX, randomY, 0f);

        StartCoroutine(AnimateArcMovement());
    }

    IEnumerator AnimateArcMovement()
    {
        float totalTime = 0.5f;
        float elapsed = 0f;
        float peakHeight = Random.Range(1.0f, 1.6f);

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            float step = Mathf.Clamp01(elapsed / totalTime);
            
            Vector3 groundLinearPath = Vector3.Lerp(baselineOrigin, targetDropFloor, step);
            float currentArcHeight = Mathf.Sin(step * Mathf.PI) * peakHeight;

            transform.position = new Vector3(groundLinearPath.x, groundLinearPath.y + currentArcHeight, groundLinearPath.z);
            yield return null;
        }

        elapsed = 0f;
        float bounceTime = 0.2f;
        Vector3 bounceOrigin = transform.position;
        float bouncePeakHeight = peakHeight * 0.25f;

        while (elapsed < bounceTime)
        {
            elapsed += Time.deltaTime;
            float step = Mathf.Clamp01(elapsed / bounceTime);
            
            Vector3 groundLinearPath = Vector3.Lerp(bounceOrigin, targetDropFloor, step);
            float currentArcHeight = Mathf.Sin(step * Mathf.PI) * bouncePeakHeight;

            transform.position = new Vector3(groundLinearPath.x, groundLinearPath.y + currentArcHeight, groundLinearPath.z);
            yield return null;
        }

        transform.position = targetDropFloor;
        canCollect = true;
        StartCoroutine(AnimateIdleWobble());
    }

    IEnumerator AnimateIdleWobble()
    {
        Vector3 landPosition = transform.position;
        while (canCollect)
        {
            animationAge += Time.deltaTime * 3.5f;
            float floatingOffset = Mathf.Sin(animationAge) * 0.08f;
            transform.position = new Vector3(landPosition.x, landPosition.y + floatingOffset, landPosition.z);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (!canCollect) return;

        int finalPayout = coinValue;

        if (AnimalStarsManager.Instance != null)
        {
            float starMultiplier = AnimalStarsManager.Instance.GetCoinMultiplier();
            finalPayout = Mathf.RoundToInt(coinValue * starMultiplier);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRewards(finalPayout, BaseXpReward);
        }
        
        Destroy(gameObject);
    }
}