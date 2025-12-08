using UnityEngine;

public class FinalPuzzle : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public float interactRange = 4f;   // how close the player must be
    Transform playerTransform;
    public bool inAltarRange;

    [Header("Puzzle State")]
    public int finalCandleCount;
    public bool allCandlesLit;

    public bool rosePlaced;
    public GameObject roses;

    public bool watchPlaced;
    public GameObject watch;

    public bool toyPlaced;
    public GameObject toy;

    public bool pendantPlaced;
    public GameObject pendant;

    public GameObject finalKey;
    bool keySpawned;

    [Header("UI")]
    public GameObject finalPuzzlePanel;
    bool altarBookOpen;

    public InventoryController InventoryController;

    void Awake()
    {
        InventoryController = FindFirstObjectByType<InventoryController>();
        Debug.Log("Final puzzle altar script on " + gameObject.name);

        finalCandleCount = 0;
        allCandlesLit = false;

        if (finalPuzzlePanel != null)
            finalPuzzlePanel.SetActive(false);

        altarBookOpen = false;

        if (toy != null) toy.SetActive(false);
        if (watch != null) watch.SetActive(false);
        if (pendant != null) pendant.SetActive(false);
        if (roses != null) roses.SetActive(false);
        finalKey.SetActive(false);

        watchPlaced = false;
        pendantPlaced = false;
        rosePlaced = false;
        toyPlaced = false;

        inAltarRange = false;
    }

    void Start()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }

    void Update()
    {
        if (finalCandleCount >= 6)
        {
            allCandlesLit = true;
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && altarBookOpen)       //both enters work
        {
            if (finalPuzzlePanel != null)
                finalPuzzlePanel.SetActive(false);

            altarBookOpen = false;
        }

        UpdateAltarRange();
        SpawnFinalKey();      //waits for conditions to be met to spawn final key
    }

    void UpdateAltarRange()
    {
        if (playerTransform == null)
        {
            inAltarRange = false;
            return;
        }

        float distanceToAltar = Vector3.Distance(playerTransform.position, transform.position);
        inAltarRange = distanceToAltar <= interactRange;
    }

    bool IsPlayerInRange()
    {
        UpdateAltarRange();
        return inAltarRange;
    }

    public void Interact(PlayerController playerController)
    {
        if (!IsPlayerInRange())
        {
            FeedbackBanner.Instance.Show("Foreboding... But now is not the time to keep my distance, I need to get closer.");
            return;
        }

        else { FeedbackBanner.Instance.Show("The altar reacts to my presence... Time to complete the ritual."); }
        //OpenAltarBook();
    }

    public void OpenAltarBook()
    {
        if (finalPuzzlePanel != null)
        {
            finalPuzzlePanel.SetActive(true);
            altarBookOpen = true;
            InventoryController.TurnOffFlashlight();
        }
    }

    public void PlaceRoses()
    {
        FeedbackBanner.Instance.Show("Beautiful roses, a symbol of deep everlasting love. A lovely gift from Briar Rose.");
        if (roses != null)
            roses.SetActive(true);

        rosePlaced = true;
    }

    public void PlacePendant()
    {
        FeedbackBanner.Instance.Show("A delicate raven-etched pendant, a symbol of the duality of nature. Dearly loved by the woman of the house; an enchanting gift from Rosalin.");
        if (pendant != null)
            pendant.SetActive(true);

        pendantPlaced = true;
    }

    public void PlaceWatch()
    {
        FeedbackBanner.Instance.Show("An antique gentleman's watch, frozen in time. A symbol of wisdom and duality -- powerful gift from Rowan.");
        if (watch != null)
            watch.SetActive(true);

        watchPlaced = true;
    }

    public void PlaceToy()
    {
        FeedbackBanner.Instance.Show("The cat's toy, such a cute and playful little guy. A whimsical gift from Kitty.");
        toy.SetActive(true);
        toyPlaced = true;
    }

    public void SpawnFinalKey()
    {
        if (!keySpawned && allCandlesLit && toyPlaced && watchPlaced && rosePlaced && pendantPlaced)          //probs could have done this better but whatever
        {
            finalKey.SetActive(true);
            FeedbackBanner.Instance.Show("A key materializes on the altar, glowing with an ethereal light. This must be the final key for escape!");
            keySpawned = true;
        }
    }
}
