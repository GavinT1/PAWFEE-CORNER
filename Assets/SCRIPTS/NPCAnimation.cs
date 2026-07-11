using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    private Animator anim;
    private Vector3 lastPosition;

    public float movementThreshold = 0.01f;

    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        if (anim == null) return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > movementThreshold)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        lastPosition = transform.position;
    }
}