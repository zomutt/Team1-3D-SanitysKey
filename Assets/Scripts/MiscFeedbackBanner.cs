using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class MiscFeedbackBanner : MonoBehaviour
{
    public static MiscFeedbackBanner Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] TMP_Text messageText;   // feedback text (parent)
    [SerializeField] Image backgroundImage;  // the image behind it (child)

    [Header("Timing")]
    [SerializeField] float defaultHoldSeconds = 2f;   // how long it stays fully visible
    [SerializeField] float defaultFadeSeconds = 1.2f; // fade-out duration

    [Header("Background Sizing")]
    [SerializeField] Vector2 backgroundPadding = new Vector2(40f, 20f); // x = left+right, y = top+bottom
    [SerializeField] float minTextWidth = 150f;
    [SerializeField] float maxTextWidth = 1600f;


    Coroutine activeRoutine;

    void Awake()
    {
        // simple singleton so other scripts can call FeedbackBanner.Instance.Show(...)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // auto-hook in case we forgor in inspector
        if (messageText == null) messageText = GetComponent<TMP_Text>();
        if (backgroundImage == null) backgroundImage = GetComponentInChildren<Image>();

        SetAlpha(0f); // starts hidden
    }

    public void Show(string text, float holdSeconds = -1f, float fadeSeconds = -1f)
    {
        if (holdSeconds < 0f) holdSeconds = defaultHoldSeconds;
        if (fadeSeconds < 0f) fadeSeconds = defaultFadeSeconds;

        // if another message is mid-fade, stop it
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ShowRoutine(text, holdSeconds, fadeSeconds));
    }

    IEnumerator ShowRoutine(string text, float holdSeconds, float fadeSeconds)
    {
        // set text and snap visible
        if (messageText != null) messageText.text = text;
        ResizeBackgroundToText();
        SetAlpha(1f);

        // stay fully visible
        yield return new WaitForSeconds(holdSeconds);

        // fade out
        float elapsedTime = 0f;
        while (elapsedTime < fadeSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeSeconds);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        activeRoutine = null;
    }
    void ResizeBackgroundToText()
    {
        if (messageText == null || backgroundImage == null) return;

        RectTransform textRect = messageText.rectTransform;
        RectTransform backgroundRect = backgroundImage.rectTransform;

        // Make sure TMP is up to date
        messageText.ForceMeshUpdate();

        // 1) How wide would the text like to be on one line?
        Vector2 preferredSingleLine = messageText.GetPreferredValues(messageText.text);
        float targetTextWidth = Mathf.Clamp(preferredSingleLine.x, minTextWidth, maxTextWidth);

        // 2) Apply that width to the text RectTransform so wrapping uses it
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetTextWidth);

        // 3) Now that width is fixed, find the height needed with wrapping
        messageText.ForceMeshUpdate();
        Vector2 preferredWrapped = messageText.GetPreferredValues(messageText.text, targetTextWidth, 0f);
        float targetTextHeight = preferredWrapped.y;

        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetTextHeight);

        // 4) Set background size = text size + padding
        float backgroundWidth = targetTextWidth + backgroundPadding.x;
        float backgroundHeight = targetTextHeight + backgroundPadding.y;

        backgroundRect.sizeDelta = new Vector2(backgroundWidth, backgroundHeight);

        // Optional: make sure background is aligned with the text
        backgroundRect.anchorMin = textRect.anchorMin;
        backgroundRect.anchorMax = textRect.anchorMax;
        backgroundRect.pivot = textRect.pivot;
        backgroundRect.anchoredPosition = textRect.anchoredPosition;
    }


    void SetAlpha(float alpha)
    {
        if (messageText != null)
        {
            Color c = messageText.color;
            c.a = alpha;
            messageText.color = c;
        }

        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = alpha;
            backgroundImage.color = c;
        }
    }
}
