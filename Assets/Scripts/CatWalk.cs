using UnityEngine;
using ithappy.Animals_FREE;

public class CatWalk : MonoBehaviour
{
    public CreatureMover creatureMover;
    public Transform destination;       //the mirror's position
    public float stopDistance = 0.5f;

    private bool isWalking;

    private void Awake()
    {
        if (creatureMover == null)
        {
            creatureMover = GetComponent<CreatureMover>();
        }
    }

    // this is called from other scripts to start walking
    public void StartWalkToDestination()
    {
        Debug.Log("Cat walking to mirror.");
        if (destination == null) return;
        isWalking = true;
    }

    private void Update()
    {
        if (!isWalking || destination == null) return;

        Vector3 toTarget = destination.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= stopDistance)
        {
            // Close enough -> stop
            creatureMover.SetInput(Vector2.zero,
                transform.position + transform.forward,
                isRun: false,
                isJump: false);
            isWalking = false;
            return;
        }

        Vector3 direction = toTarget.normalized;

        // Always move "forward" relative to where we want to go
        Vector2 axis = new Vector2(0f, 1f);
        Vector3 lookTarget = transform.position + direction;

        creatureMover.SetInput(axis, lookTarget, isRun: false, isJump: false);
    }
}
