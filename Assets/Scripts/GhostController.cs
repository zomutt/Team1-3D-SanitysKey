using System.Collections;
using UnityEngine;
using TMPro;

public class GhostController : MonoBehaviour
{
    public int candlesLit;
    public GameObject ghostGirl;
    public PlayerController PlayerController;
    public bool tombstoneInteracted;
    public TextMeshProUGUI ghostText;
    public bool ghostAwakened;
    public GameObject ladder;
    
    private void Awake()
    {
        tombstoneInteracted = false;
        ghostText.text = "";
        ghostGirl.SetActive(false);
        //ladder.SetActive(false);

        // Auto-assign PlayerController
        if (PlayerController == null)
        {
            PlayerController = PlayerController.Instance;

            if (PlayerController == null)
            {
                Debug.LogWarning("CandleController could not find PlayerController.Instance.");
            }
        }
    }
    void Update()
    {
        if (candlesLit > 5 && tombstoneInteracted) { AwakenGirl(); ghostAwakened = true; }
    }

    void AwakenGirl()
    {
        candlesLit = 0;   //prevents it from happening again,, on other script candles are still designated as "lit" so nothing can happen
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2.5f);
        ghostGirl.SetActive(true);
        FeedbackBanner.Instance.Show("Could that be little Briar Rose herself..?");
        PlayerController.pSanity -= 35;
        yield return new WaitForSeconds(2.5f);
        FeedbackBanner.Instance.Show("She seems... harmless. Quite unlike anything else found here.");
    }
}
