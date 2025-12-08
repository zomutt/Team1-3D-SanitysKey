using TMPro;
using UnityEngine;
using System.Collections;

public class SafeManager : MonoBehaviour, IInteractable
{
    public GameObject safePanel;
    [SerializeField] TMP_InputField combinationInput;
    [SerializeField] TextMeshProUGUI feedbackText;
    public float fadeDuration = 1.0f;
    [SerializeField] string correctCombination = "1225";
    bool safePanelOpen;
    [SerializeField] GameObject key;
    public InventoryController InventoryController;
    public GameObject watch;
    bool hasOpened;
    PlayerController PlayerController;
    public AudioSource AudioSource;
    public AudioClip scream;
    bool canOpen;

    void Awake()
    {
        PlayerController = FindFirstObjectByType<PlayerController>();
        InventoryController = FindFirstObjectByType<InventoryController>();
        safePanel.SetActive(false);
        key.SetActive(false);
        watch.SetActive(false);
        hasOpened = false;
        safePanelOpen = false;
        canOpen = true;
    }
    public void Interact(PlayerController PlayerController)
    {
        Debug.Log("Interacting with Safe");
        if (canOpen)
        { SafeSequence(); }
        if (!canOpen) { return; Debug.Log("Error: cannot open"); }
    }
    void SafeSequence()
    {
        if (canOpen)
        {
            safePanel.SetActive(true);
            safePanelOpen = true;
            PlayerController.Instance.UnlockMouse();
            InventoryController.canUseInv = false;
        }
    }

    public void OnClickSubmit()
    {
        if (!hasOpened)
        {
            if (combinationInput.text == correctCombination)
            {
                key.SetActive(true);
                watch.SetActive(true);
                Debug.Log("Safe Unlocked!");
                feedbackText.text = "Unlocked!";
                hasOpened = true;
                gameObject.tag = "Untagged";
            }
            else
            {
                Debug.Log("Incorrect Combination. Try Again.");
                ShowAndFade("Failed.");
                PlayerController.pSanity -= 10;
                combinationInput.text = "";
                AudioSource.PlayOneShot(scream);
            }
        }
    }

    public void OnClickClose()
    {
        safePanel.SetActive(false);
        safePanelOpen = false;
        PlayerController.Instance.LockMouse();
        InventoryController.canUseInv = true;
    }

    public void ShowAndFade(string text)
    {
        feedbackText.text = text;
        feedbackText.alpha = 1.0f;
        StartCoroutine(FadeOutFeedback());
    }

    private IEnumerator FadeOutFeedback()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            feedbackText.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
            yield return null;
        }
        feedbackText.alpha = 0.0f;
    }
}
