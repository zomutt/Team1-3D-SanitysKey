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
    bool hasOpened;

    void Awake()
    {
        InventoryController = FindObjectOfType<InventoryController>();
        safePanel.SetActive(false);
        key.SetActive(false);
        hasOpened = false;
    }
    public void Interact(PlayerController PlayerController)
    {
        SafeSequence();
    }
    void SafeSequence()
    {
        safePanel.SetActive(true);
        safePanelOpen = true;
        PlayerController.Instance.UnlockMouse();
        InventoryController.canUseInv = false;
    }

    public void OnClickSubmit()
    {
        if (!hasOpened)
        {
            if (combinationInput.text == correctCombination)
            {
                key.SetActive(true);
                Debug.Log("Safe Unlocked!");
                feedbackText.text = "Unlocked!";
                hasOpened = true;
            }
            else
            {
                Debug.Log("Incorrect Combination. Try Again.");
                ShowAndFade("Failed.");
                combinationInput.text = "";
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
