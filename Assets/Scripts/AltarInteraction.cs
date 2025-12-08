using UnityEngine;

public class AltarInteraction : MonoBehaviour, IInteractable
{
    public float interactRange = 4f;   // how close the player must be

    Transform playerTransform;

    void Start()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }

    bool IsPlayerInRange()
    {
        if (playerTransform == null) return false;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        return distance <= interactRange;
    }

    public void Interact(PlayerController playerController)
    {
        if (!IsPlayerInRange())
        {
            FeedbackBanner.Instance.Show("Foreboding... But now is not the time to keep my distance, I need to get closer.");
            return;
        }

        // Do altar stuff here
        FeedbackBanner.Instance.Show("The altar reacts to my presence... Time to complete the ritual.");
    }
}