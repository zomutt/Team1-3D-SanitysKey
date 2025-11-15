using UnityEngine;
using UnityEngine.UI;

public enum CrosshairState
{
    Default,
    Interactable
}

public class CrosshairController : MonoBehaviour
{
    public Image crosshairImage;
    public Sprite defaultSprite;
    public Sprite interactSprite;

    [Header("Interact Prompt")]
    [SerializeField] GameObject interactPrompt; // LMB tutorial image
    [SerializeField] bool showInteractHint = true;

    void Awake()
    {
        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false); // start hidden
        }
    }

    public void SetState(CrosshairState state)
    {
        if (crosshairImage == null) return;

        if (state == CrosshairState.Default)
        {
            crosshairImage.sprite = defaultSprite;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
        else // Interactable
        {
            crosshairImage.sprite = interactSprite;

            if (interactPrompt != null && showInteractHint)
                interactPrompt.SetActive(true);
        }
    }

    // Call this later (e.g. after tutorial area) to disable hints:
    public void DisableInteractHint()
    {
        showInteractHint = false;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}
