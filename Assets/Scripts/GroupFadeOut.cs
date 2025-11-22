//using UnityEngine;
//using System.Collections;

//public class GroupFadeOut : MonoBehaviour
//{
//    public float fadeDurationSeconds = 1f;
//    public bool destroyAfterFade = true;

//    Renderer[] renderersToFade;
//    Color[] startingColors;
//    bool isFading = false;

//    void Awake()
//    {
//        // Grab ALL renderers on this object and its children
//        renderersToFade = GetComponentsInChildren<Renderer>();

//        startingColors = new Color[renderersToFade.Length];

//        for (int rendererIndex = 0; rendererIndex < renderersToFade.Length; rendererIndex++)
//        {
//            startingColors[rendererIndex] = renderersToFade[rendererIndex].material.color;
//        }
//    }

//    public void StartFade()
//    {
//        if (isFading) return;
//        if (renderersToFade == null || renderersToFade.Length == 0) return;

//        StartCoroutine(FadeRoutine());
//    }

//    IEnumerator FadeRoutine()
//    {
//        isFading = true;
//        float elapsedSeconds = 0f;

//        while (elapsedSeconds < fadeDurationSeconds)
//        {
//            elapsedSeconds += Time.deltaTime;
//            float fadeProgress = Mathf.Clamp01(elapsedSeconds / fadeDurationSeconds);
//            float alphaValue = Mathf.Lerp(1f, 0f, fadeProgress);

//            for (int rendererIndex = 0; rendererIndex < renderersToFade.Length; rendererIndex++)
//            {
//                Renderer rendererToFade = renderersToFade[rendererIndex];
//                Color newColor = startingColors[rendererIndex];
//                newColor.a = alphaValue;
//                rendererToFade.material.color = newColor;
//            }

//            yield return null;
//        }

//        if (destroyAfterFade)
//        {
//            Destroy(gameObject);          // destroys parent + all fire children
//        }
//        else
//        {
//            gameObject.SetActive(false);  // hide whole fire group
//        }
//    }
//}
