using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class FeedbackBanner : MonoBehaviour
{
    public static FeedbackBanner Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] TMP_Text messageText;   // feedback text (parent)
    [SerializeField] Image backgroundImage;  // the image behind it (child)

    [Header("Timing")]
    [SerializeField] float defaultHoldSeconds = 2f;   // how long it stays fully visible
    [SerializeField] float defaultFadeSeconds = 1.2f; // fade-out duration

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
