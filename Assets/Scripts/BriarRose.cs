using UnityEngine;
using TMPro;
using System.Collections;

public class BriarRose : MonoBehaviour
{
    /// <summary>
    /// NOTE: THIS IS ON THE INTERACT SPOT OBJECT
    /// </summary>
    public static BriarRose Instance { get; private set; }

    int dialogueStep;
    public TextMeshProUGUI ghostText;
    [Header("Fade Settings")]
    public float fadeDurationSeconds = 0.5f;
    public float holdDurationSeconds = 5f;

    Coroutine activeDialogueRoutine;
    public bool inRoseRange;
    public GameObject ladder;
    bool canChat;



    public InventoryController InventoryController;
    private void Awake()
    {
        dialogueStep = 0;
        if (ghostText != null)         //fancy ass fade in fade out mechanic for some smooth dialogue
        {
            Color startColor = ghostText.color;
            startColor.a = 0f;          
            ghostText.color = startColor;
            ghostText.text = "";
        }

        Instance = this;

        dialogueStep = 0;
        if (ghostText != null)
        {
            Color startColor = ghostText.color;
            startColor.a = 0f;
            ghostText.color = startColor;
            ghostText.text = "";
        }
    }

    private void OnEnable()
    {
        activeDialogueRoutine = StartCoroutine(FadeDialogue("Oh, hi. Can you hear them too? The whispers? I can tell you're not one of them. Perhaps you can help me? I can provide what you need in kind."));
        StartCoroutine(ChatCD()); 
        dialogueStep++;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) { inRoseRange = true; Debug.Log("inRoseRange: " + inRoseRange); }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            inRoseRange = false;
            Debug.Log("Player collided");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player collided: OTEnter");
        inRoseRange = true;

        // If she is already talking OR on cooldown, ignore this bump
        if (activeDialogueRoutine != null || !canChat)
            return;

        // FROM HERE ON: safe to start a new line
        if (dialogueStep == 0)       // fallback 1st go
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "Oh, hi. Can you hear them too? The whispers? I can tell you're not one of them."));
            dialogueStep++;
            StartCoroutine(ChatCD());
        }
        else if (dialogueStep == 1)
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "Mama still keeps something dear to us close to her, but I can't reach it."));
            dialogueStep++;
            StartCoroutine(ChatCD());
        }
        else if (dialogueStep == 2)
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "Mama's slumber is also restless... Perhaps helping us can also bring peace to yourself."));
            dialogueStep++;
            StartCoroutine(ChatCD());
        }
        else if (dialogueStep == 3)
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "They lay us out for eternal rest, yet only one of us remembers how to bloom."));
            dialogueStep = 1;
            StartCoroutine(ChatCD());
        }
        else if (dialogueStep == 4 || dialogueStep == 5)
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "You've gotten me my roses, thank you. Please, hurry and leave before the whispers consume you as well. Don't forget to hold your own rose close when the whispers close in."));
            dialogueStep++;
            StartCoroutine(ChatCD());
        }
        else if (dialogueStep == 6)
        {
            activeDialogueRoutine = StartCoroutine(FadeDialogue(
                "No, voices... no, stay away from her... she's kind."));
            dialogueStep = 4;
            StartCoroutine(ChatCD());
        }
    }

    IEnumerator ChatCD()       //makes sure dialogue goes linearly and cannot be accidentally skipped
    {
        canChat = false;
        yield return new WaitForSeconds(5f);
        canChat = true;
    }
    public void TurnInRose()
    {
        Debug.Log("BR script: TurnInRose triggered");

        activeDialogueRoutine = StartCoroutine(FadeDialogueLong(
            "Yes! Yes! Roses from mama! Thank you! Here, I'll show you the way out. Turn around, a hidden ladder will appear. Please, hurry and leave before the whispers consume you as well. And please... keep a rose. They provide me with comfort in the dark, maybe one can help you too."));

        StartCoroutine(ChatCD());
        ladder.SetActive(true);
        dialogueStep = 4;
    }



    IEnumerator FadeDialogue(string line)
    {
        if (ghostText == null) yield break;

        Color textColor = ghostText.color;
        textColor.a = 0f;
        ghostText.color = textColor;
        ghostText.text = line;

        // fade in
        float elapsedSeconds = 0f;
        while (elapsedSeconds < fadeDurationSeconds)
        {
            elapsedSeconds += Time.deltaTime;
            float progress = elapsedSeconds / fadeDurationSeconds;
            textColor.a = Mathf.Lerp(0f, 1f, progress);
            ghostText.color = textColor;
            yield return null;
        }
        textColor.a = 1f;
        ghostText.color = textColor;

        // hold
        yield return new WaitForSeconds(10f);

        // fade out
        elapsedSeconds = 0f;
        while (elapsedSeconds < fadeDurationSeconds)
        {
            elapsedSeconds += Time.deltaTime;
            float progress = elapsedSeconds / fadeDurationSeconds;
            textColor.a = Mathf.Lerp(1f, 0f, progress);
            ghostText.color = textColor;
            yield return null;
        }
        textColor.a = 0f;
        ghostText.color = textColor;
        ghostText.text = "";

        activeDialogueRoutine = null;
    }

    IEnumerator FadeDialogueLong(string line)
    {
        if (ghostText == null) yield break;

        Color textColor = ghostText.color;
        textColor.a = 0f;
        ghostText.color = textColor;
        ghostText.text = line;

        // fade in
        float elapsedSeconds = 0f;
        while (elapsedSeconds < fadeDurationSeconds)
        {
            elapsedSeconds += Time.deltaTime;
            float progress = elapsedSeconds / fadeDurationSeconds;
            textColor.a = Mathf.Lerp(0f, 1f, progress);
            ghostText.color = textColor;
            yield return null;
        }
        textColor.a = 1f;
        ghostText.color = textColor;

        // hold
        yield return new WaitForSeconds(20f);

        // fade out
        elapsedSeconds = 0f;
        while (elapsedSeconds < fadeDurationSeconds)
        {
            elapsedSeconds += Time.deltaTime;
            float progress = elapsedSeconds / fadeDurationSeconds;
            textColor.a = Mathf.Lerp(1f, 0f, progress);
            ghostText.color = textColor;
            yield return null;
        }
        textColor.a = 0f;
        ghostText.color = textColor;
        ghostText.text = "";

        activeDialogueRoutine = null;
    }
}
