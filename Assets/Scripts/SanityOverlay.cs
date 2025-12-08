using UnityEngine;
using UnityEngine.UI;

public class SanityOverlay : MonoBehaviour
{
    /// <summary>
    /// full disclosure, this is fully off the internet except for me tweaking the values and making it fit. but it works nicely and is p neat :)
    /// </summary>
    [Header("References")]
    [SerializeField] Image overlayImage;

    [Header("Sanity thresholds")]
    [SerializeField] float startFadeSanity = 90f;  // overlay starts at or below this sanity
    [SerializeField] float fullFadeSanity = 5f;   // overlay maxes out at this sanity
    [SerializeField] float maxAlpha = 1f;   // strongest opacity

    // how visible it is *right when* sanity hits 70
    [SerializeField] float minAlphaAtStart = 0.1f;

    [Header("Fade smoothing")]
    [SerializeField] float fadeSpeed = 5f;

    [Header("Pulse settings")]
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float pulseAmount = 0.3f;

    void Awake()
    {
        if (overlayImage == null)
            overlayImage = GetComponent<Image>();

        SetAlpha(0f);
    }

    void Update()
    {
        if (overlayImage == null || PlayerController.Instance == null)
            return;

        float currentSanity = PlayerController.Instance.pSanity;

        float t = 0f;
        float baseAlpha = 0f;

        // Only start showing when we are at or below the start threshold
        if (currentSanity <= startFadeSanity)
        {
            // t = 0 at 70%, t = 1 at 5%
            t = Mathf.InverseLerp(startFadeSanity, fullFadeSanity, currentSanity);

            // base alpha goes from minAlphaAtStart (at 70%) to maxAlpha (at 5%)
            baseAlpha = Mathf.Lerp(minAlphaAtStart, maxAlpha, t);
        }

        float targetAlpha = baseAlpha;

        if (t > 0f)
        {
            // Pulse 0..1
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f) + 0.5f;

            // Scale between 1 and (1 + pulseAmount)
            float pulseScale = Mathf.Lerp(1f, 1f + pulseAmount, pulse);

            targetAlpha = baseAlpha * pulseScale;
            targetAlpha = Mathf.Clamp(targetAlpha, 0f, maxAlpha);
        }

        Color color = overlayImage.color;
        float newAlpha = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
        color.a = newAlpha;
        overlayImage.color = color;
    }

    void SetAlpha(float alpha)
    {
        if (overlayImage == null) return;
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}
