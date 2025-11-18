using System.Collections;
using UnityEngine;

public class LightSensitiveEntity : MonoBehaviour
{
    // This script is meant for entities that are sensitive to light.
    // "isFlashed" should be set TRUE by whatever detects the flashlight.

    [Header("Health")]
    public float entityHP = 10f;          // set this in Inspector
    public float damagePerFlash = 2f;     // how much HP is lost per "hit" of light
    public bool isFlashed;                // set from PC script

    [Header("Damage Timing")]
    public float iframeCooldown = 0.75f;  // delay between damage ticks
    bool canTakeDamage = true;

    [Header("Fade Out")]
    public Renderer entityRenderer;       // assign in Inspector (main mesh)
    public float fadeDurationSeconds = 1.5f;

    bool isFading;

    void Start()
    {
        // Safety: if you forget to assign, try to grab a renderer on this object
        if (entityRenderer == null)
        {
            entityRenderer = GetComponentInChildren<Renderer>();
        }
    }

    void Update()
    {
        // While the entity is being flashed and is allowed to take damage,
        // subtract HP once, then start an iframe cooldown.
        if (isFlashed && canTakeDamage && !isFading)
        {
            entityHP -= damagePerFlash;

            if (entityHP <= 0f)
            {
                StartCoroutine(FadeAndDisable());
            }

            StartCoroutine(IFrameRoutine());
        }
    }

    IEnumerator IFrameRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(iframeCooldown);
        canTakeDamage = true;
    }

    IEnumerator FadeAndDisable()
    {
        isFading = true;

        if (entityRenderer == null)
        {
            gameObject.SetActive(false);
            yield break;     //stop coroutine and prevent coming back to it
        }

        Material entityMaterial = entityRenderer.material;
        Color originalColor = entityMaterial.color;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDurationSeconds)
        {
            float timePercent = elapsedTime / fadeDurationSeconds;
            float alphaValue = Mathf.Lerp(1f, 0f, timePercent);

            Color fadedColor = originalColor;
            fadedColor.a = alphaValue;
            entityMaterial.color = fadedColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // makes sure it is fully invisible at the end
        Color finalColor = originalColor;
        finalColor.a = 0f;
        entityMaterial.color = finalColor;

        gameObject.SetActive(false);
    }
}
