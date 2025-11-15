using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class HurtOverlay : MonoBehaviour
{
    [SerializeField] Image overlayImage;
    [SerializeField] float fadeInDuration = 0.1f;
    [SerializeField] float holdDuration = 0.1f;
    [SerializeField] float fadeOutDuration = 0.3f;
    [SerializeField] float maxAlpha = 0.7f;   // how strong the flash gets

    Coroutine activeRoutine;

    void Awake()
    {
        if (overlayImage == null)
        {
            overlayImage = GetComponent<Image>();
        }

        SetAlpha(0f); // start invisible
    }

    public void Play()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, elapsedTime / fadeInDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(maxAlpha);

        // Hold at full
        yield return new WaitForSecondsRealtime(holdDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, elapsedTime / fadeOutDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        activeRoutine = null;
    }

    void SetAlpha(float alpha)
    {
        if (overlayImage == null) return;

        Color color = overlayImage.color;
        color.a = alpha;
        overlayImage.color = color;
    }
}

