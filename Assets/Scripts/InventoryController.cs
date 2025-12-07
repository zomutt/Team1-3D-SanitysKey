using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

public class InventoryController : MonoBehaviour
{

    /// <summary>
    ///  This script is used to control ANY item the character uses,, i.e. flashlight, med packs, etc.
    /// </summary>


    public bool flashlightOn;
    public int medCharges;      //med packs can stack so player can have more than 1
    public int bulbCharges;
    public int bulbRestore = 35;
    public int laudanumCharges;
    public int laudanumRestore = 35;
    public bool hasMatches;
    public float flLife;
    public bool hasCanteen;
    public bool flWarningTextShown;
    public bool flOutTextShown;
    public TextMeshProUGUI flLifeDisplay;
    public TextMeshProUGUI medChargeDisplay; 
    public TextMeshProUGUI bulbChargeDisplay;
    public TextMeshProUGUI laudanumChargeDisplay;
    public TextMeshProUGUI canteenChargeDisplay;
    public GameObject flDirectionalLight;
    public int medpackHeal = 30;
    public int canteenCount;
    public int filledCanteenCount;
    public bool inTubRange;
    public PlayerController PlayerController;
    public GameObject flashlightCone;
    public GameObject canteenImg;
    public GameObject matchImg;
    public GameObject fireParent;
    [HideInInspector] public bool inRangeCanteen;
    [SerializeField] AudioClip sizzleFire;

    public bool hasToy;
    bool canPlay = true;
    public GameObject toyImg;
    public bool inCatRange;
    bool hasPlayed;
    public CatWalk catWalk;
    


    public bool canUseInv;   //sometimes the num keys should be disabled, e.g. during cutscenes, certain quests, etc.

    [Header("Audio")]
    public AudioClip fireSuccess;
    public AudioClip noneed;
    public AudioClip lightout;
    public AudioClip lightlow;
    public AudioClip lightfail;
    public AudioClip laudanumsuccess;
    public AudioClip laudanumfail;
    public AudioClip laudanumfull;
    public AudioClip healsuccess;
    public AudioClip healfail;
    public AudioClip canteenfail;
    public AudioClip bulbout;
    public AudioClip flashlightSFX;
    

    AudioSource audioSource;

    [Header("GhostPuzzle")]
    public bool hasRose;
    public GameObject roseImg;
    bool canRose;
    float roseCD = 90f;
    public BriarRose BriarRose;

    public GameObject helpPanel;
    bool helpOpen;
    private void Awake()
    {
        catWalk = FindFirstObjectByType<CatWalk>();
        audioSource = GetComponent<AudioSource>();
        flashlightOn = false;
        flDirectionalLight.SetActive(false);
        flWarningTextShown = false;
        flOutTextShown = false;
        hasMatches = false;
        flLife = 100; //starts player with full charge bc we're nice :)
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            medCharges = 2;   //test purposes,, ensures we have enough to test stuff without giving them to us on other levels. i want players to have to find them.
        }
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            medCharges = 0;
            bulbCharges = 0;
            laudanumCharges = 0;
            canteenCount = 0;
        }
        //sconeText.text = "";
        helpOpen = false;
        hasToy = false;
        helpPanel.SetActive(false);
        roseImg.SetActive(false);
        toyImg.SetActive(false);
        canUseInv = true;
        canPlay = true;
        hasPlayed = false;
    }

    private void Update()
    {
        string medChText = medCharges.ToString("F0");      //f = fixed-point format (reg decimal number), 0 = amt of decimal places to show,, kinda irrelevant here but i was sleep deprived. good practice for this at least. :) 
        string flLifeText = flLife.ToString("F0");
        string bulbChargeText = bulbCharges.ToString("F0");
        string laudinumChargeText = laudanumCharges.ToString("F0");
        string canteenCountText = canteenCount.ToString();
        medChargeDisplay.text = "(" + medChText + ")";
        flLifeDisplay.text = flLifeText + "%";
        bulbChargeDisplay.text = "(" + bulbChargeText + ")";
        laudanumChargeDisplay.text = "(" + laudinumChargeText + ")";
        canteenChargeDisplay.text = "(" + canteenCountText + ")";
        //lights1.SetActive(false); lights2.SetActive(false); lights3.SetActive(false); lights4.SetActive(false);
        if (flLife < 0) flLife = 0;
        if (flLife >= 100) flLife = 100;
        if (!hasMatches) { matchImg.SetActive(false); }
        if (hasMatches) { matchImg.SetActive(true); }
        if (hasToy) { toyImg.SetActive(true); }

        if (flashlightOn)
        {
            flLife -= .25f * Time.deltaTime;   // drain per second,, so approx 1 charge every 4 sec is drained. tweak as needed. for whatever reason doesn't work if assigned in inspector. god hates us.
        }

        if (flLife > 25)
        {
            flWarningTextShown = false;     //makes sure warning text only appears once
            flOutTextShown = false;
        }
        else if (flLife < 26 && flLife > 0)
        {
            if (!flWarningTextShown)
            {
                FeedbackBanner.Instance.Show("I need to find a bulb.");
                audioSource.PlayOneShot(lightlow);
                flWarningTextShown = true;
            }
            flWarningTextShown = true;
        }
        else if (flLife < 1)    //turns off fl when life = 0,, keep value at one bc the time operates in decimals even if doesnt show
        {
            flDirectionalLight.SetActive(false);
            if (!flOutTextShown)
            {
                FeedbackBanner.Instance.Show("I should have been looking for a bulb...");
                audioSource.PlayOneShot(lightout);
                flWarningTextShown = true;
            }
        }
        if (canUseInv)
        {
            if (Input.GetKeyDown("1"))
            {
                if (flLife < 1)
                {
                    FeedbackBanner.Instance.Show("The bulb on this is out.");
                    audioSource.PlayOneShot(lightfail);
                }
                else if (!flashlightOn && flLife > 1)   // turn ON
                {
                    flDirectionalLight.SetActive(true);
                    flashlightCone.SetActive(true);
                    audioSource.PlayOneShot(flashlightSFX);
                    flashlightOn = true;
                    Debug.Log("Turning ON FL");
                }
                else if (flashlightOn)                  // turn OFF
                {
                    flDirectionalLight.SetActive(false);
                    flashlightCone.SetActive(false);
                    audioSource.PlayOneShot(flashlightSFX);
                    flashlightOn = false;
                    Debug.Log("Turning OFF FL");
                }
            }

            if (Input.GetKeyDown("2"))
            {
                if (PlayerController.pHP >= 100)
                {
                    FeedbackBanner.Instance.Show("I don't need to use that yet.");
                    audioSource.PlayOneShot(noneed);
                }
                else if ((PlayerController.pHP < 100 && medCharges > 0))
                {
                    PlayerController.pHP += medpackHeal;
                    Debug.Log("Player healed for: " + medpackHeal + " new HP: " + PlayerController.pHP);
                    FeedbackBanner.Instance.Show("Much better.");
                    audioSource.PlayOneShot(healsuccess);
                    medCharges -= 1;
                }
                else if (medCharges < 1)
                {
                    FeedbackBanner.Instance.Show("I need to find more medpacks");
                    audioSource.PlayOneShot(healfail);
                }
            }

            if (Input.GetKeyDown("3"))
            {
                if (bulbCharges < 1)
                {
                    FeedbackBanner.Instance.Show("I need to find more lightbulbs");
                    audioSource.PlayOneShot(bulbout);
                }
                else { flLife += bulbRestore; }
            }
            if (Input.GetKeyDown("4"))
            {
                if (laudanumCharges < 1)
                {
                    FeedbackBanner.Instance.Show("I need to find more laudanum.");
                    audioSource.PlayOneShot(laudanumfail);
                }
                else if (laudanumCharges > 0 && PlayerController.pSanity > 99)
                {
                    FeedbackBanner.Instance.Show("I don't need to use that yet.");
                    audioSource.PlayOneShot(laudanumfull);
                }
                else if (laudanumCharges > 0 && PlayerController.pSanity < 99)
                {
                    FeedbackBanner.Instance.Show("Ah, I feel much more grounded now.");
                    audioSource.PlayOneShot(laudanumsuccess);
                    PlayerController.pSanity += laudanumRestore;
                }
            }
            if (Input.GetKeyDown("5"))
            {
                if (hasCanteen && inRangeCanteen && (filledCanteenCount == 0))
                {
                    filledCanteenCount++; FeedbackBanner.Instance.Show("This water is gross, but I'll fill my canteen with it anyways.");
                    audioSource.PlayOneShot(PlayerController.watersuccess);
                }
                else if (filledCanteenCount > 0 && !inRangeCanteen)
                {
                    Debug.Log("In range: " + inRangeCanteen);
                    FeedbackBanner.Instance.Show("Ew... I really shouldn't drink anything I find in here.");
                    audioSource.PlayOneShot(canteenfail);
                }
                else if (filledCanteenCount > 0 && inRangeCanteen)
                {
                    FeedbackBanner.Instance.Show("Aha! I knew it. Let's check this out.");
                    canteenCount--;
                    fireParent.SetActive(false);
                    StartCoroutine(successdelay());
                }
            }
            if (Input.GetKeyDown("6"))
            {
                if (hasMatches) PlayerController.Instance.LightCandle();
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))   // same style as 1–6
            {
                Debug.Log("7 pressed in InventoryController");

                Debug.Log("Has rose: " + hasRose);

                if (!hasRose)
                {
                    Debug.Log("Cannot turn in rose: player has no rose.");
                    return;
                }

                if (BriarRose.Instance == null)
                {
                    Debug.LogWarning("No BriarRose.Instance found in scene.");
                    return;
                }

                if (!BriarRose.Instance.inRoseRange)
                {
                    Debug.Log("Cannot turn in rose: not in BriarRose range.");
                    return;
                }

                BriarRose.Instance.TurnInRose();
                Debug.Log("TurnInRose() called on BriarRose");

                if (PlayerController.inRosalinRange)
                {
                    FeedbackBanner.Instance.Show("Rosalin seems pleased with the rose.");
                    PlayerController.hasGivenRose = true;
                    MiscFeedbackBanner.Instance.Show("Thank you for bringing me my rose, you must have met my daughter... You should leave, take the pendant and key by the door and go.");
                }
            }

            if (Input.GetKeyDown("8"))
            {
                if (hasToy)
                {
                    if (inCatRange)
                    {
                        if (canPlay)
                        {
                            //audioSource.PlayOneShot(PlayerController.catpurr);
                            PlayerController.pSanity += 15;
                            StartCoroutine(catCD());
                            if (hasPlayed) { FeedbackBanner.Instance.Show("The cat seems excited to play again!"); }
                            else if (!hasPlayed) { FeedbackBanner.Instance.Show("The cat seems quite pleased, let's follow it."); catWalk.StartWalkToDestination(); }
                        }
                        else if (!canPlay) { FeedbackBanner.Instance.Show("The cat seems tired of playing for now."); }
                    }
                    else { FeedbackBanner.Instance.Show("This is cute, but I'm not a cat."); }
                }
            }
        }
    }

    private IEnumerator RoseCD()       //cd for sanity/stam restore
    {
        canRose = false;
        yield return new WaitForSeconds(roseCD);
        canRose = true;
    }
    private IEnumerator successdelay()
    {
        audioSource.PlayOneShot(sizzleFire);
        yield return new WaitForSeconds(.5f);
        audioSource.PlayOneShot(fireSuccess);
    }

    private IEnumerator catCD()
    {
        canPlay = false;
        yield return new WaitForSeconds(30f);
        canPlay = true;
    }
}