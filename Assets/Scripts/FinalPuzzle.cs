using UnityEngine;

public class FinalPuzzle : MonoBehaviour
{
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
    public bool inAltarRange;
    public GameObject finalPuzzlePanel;
    bool altarBookOpen;
    private void Awake()
    {
        finalCandleCount = 0;
        allCandlesLit = false;
        finalPuzzlePanel.SetActive(false);
        altarBookOpen = false;
        toy.SetActive(false);
        watch.SetActive(false);
        pendant.SetActive(false);
        roses.SetActive(false);
        watchPlaced = false;
        pendantPlaced = false;
        rosePlaced = false;
        toyPlaced = false;

    }

    private void Update()
    {
        if (finalCandleCount >= 6) { allCandlesLit = true; }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && altarBookOpen)
        {
            finalPuzzlePanel.SetActive(false);
            altarBookOpen = false;
        }
    }

    public void OpenAltarBook()
    {
        finalPuzzlePanel.SetActive(true);
        altarBookOpen = true;
    }

    public void PlaceRoses()
    {
        FeedbackBanner.Instance.Show("Beautiful roses, a symbol of deep everlasting love.");
        roses.SetActive(true);
        rosePlaced = true;
    }

    public void PlacePendant()
    {
        FeedbackBanner.Instance.Show("A delicate raven-etched pendant, a symbol of the duality of nature. Dearly loved by the woman of the house.");
        pendant.SetActive(true);
        pendantPlaced = true;
    }

    public void PlaceWatch()
    {
        FeedbackBanner.Instance.Show("An antique gentleman's watch, frozen in time. A symbol of wisdom and duality.");
        watch.SetActive(true);
        watchPlaced = true;
    }

    public void PlaceToy()
    {
        FeedbackBanner.Instance.Show("The cat's toy, such a cute and playful little guy.");
        toy.SetActive(true);
        toyPlaced = true;
    }
}

