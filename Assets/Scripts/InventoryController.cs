using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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
    [Header("Scones")]
    public bool inRangeS1;
    public bool inRangeS2;
    public bool inRangeS3;
    public bool inRangeS4;
    public GameObject lights1;
    public GameObject lights2;
    public GameObject lights3;
    public GameObject lights4;
    public string lightString;
    //public int lightCount;
    public GameObject sconeDoor;
    public TextMeshProUGUI sconeText;


    public GameObject helpPanel;
    bool helpOpen;
    private void Awake()
    {
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
        sconeText.text = "";
        helpOpen = false;
        helpPanel.SetActive(false);
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
        lights1.SetActive(false); lights2.SetActive(false); lights3.SetActive(false); lights4.SetActive(false);
        if (flLife < 0) flLife = 0;
        if (flLife >= 100) flLife = 100;
        if (!hasMatches) { matchImg.SetActive(false); }
        if (hasMatches) { matchImg.SetActive(true); }

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
                FeedbackBanner.Instance.Show("My light is running low. I better find a bulb soon.");
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
                FeedbackBanner.Instance.Show("I need to find a light source NOW!");
                audioSource.PlayOneShot(lightout);
                flWarningTextShown = true;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape))     //i KNOW this should go in UI controller but like for whatever reason nothing was showing up in inspector. idfk. i literally copy pasted the code over to here and it makes no gd sense. code spaghetti idc.
        //{
        //    if (!helpOpen) { helpPanel.SetActive(true); helpOpen = true; }
        //    if (helpOpen) { helpPanel.SetActive(false); helpOpen = false; }
        //}

        if (Input.GetKeyDown("1"))
        {
            if (flLife < 1)
            {
                FeedbackBanner.Instance.Show("Ugh, the bulb on this is out.");
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
                FeedbackBanner.Instance.Show("Ah, that's much better.");
                audioSource.PlayOneShot(healsuccess);
                medCharges -= 1;
            }
            else if (medCharges < 1)
            {
                FeedbackBanner.Instance.Show("I need to find more medpacks first.");
                audioSource.PlayOneShot(healfail);
            }
        }

        if (Input.GetKeyDown("3"))
        {
            if (bulbCharges < 1)
            {
                FeedbackBanner.Instance.Show("I need to find more lightbulbs first.");
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
                FeedbackBanner.Instance.Show("Thankfully I don't need to use that yet.");
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
                filledCanteenCount++; FeedbackBanner.Instance.Show("Ugh, this water is filthy! Still, I'll fill my canteen with it.");
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
    }

        //    if (Input.GetKeyDown("6"))           //*does a hail mary*,, blessed be thy father in heaven or w/e the catholics say when something unholy needs to be sanctified... or at least optimized.
        //    {
        //        //if (!hasMatches) { FeedbackBanner.Instance.Show("I can't use that yet."); }
        //        //if (hasMatches) 
        //        //{
        //            //if (inRangeS1) { lights1.SetActive(true); Debug.Log("Lit 1"); lightCount++; lightString += "1"; sconeText.text += "First scone lit... "; Debug.Log(lightString); }
        //            //else if (inRangeS2) { lights2.SetActive(true); Debug.Log("Lit 2"); lightCount++; lightString += "2"; sconeText.text += "Second scone lit... "; Debug.Log(lightString); }
        //            //else if (inRangeS3) { lights3.SetActive(true); Debug.Log("Lit 3"); lightCount++; lightString += "3"; sconeText.text += "Third scone lit... "; Debug.Log(lightString); }
        //            //else if (inRangeS4) { lights4.SetActive(true); Debug.Log("Lit 4"); lightCount++; lightString += "4"; sconeText.text += "Fourth scone lit... "; Debug.Log(lightString); }
        //            //else { Debug.Log("something wrong :("); }
        //    //    }
        //    //}
    //    if (sconeCount >= 3)         //goes w 6 i pwomise
    //    {
    //        lightCount = 0;
    //        if (lightString == "143") 
    //        {
    //            FeedbackBanner.Instance.Show("Seems that worked. Let's see what's through this door.");
    //            Debug.Log("Puzzle solved");
    //            lights2.SetActive(true); 
    //            sconeDoor.SetActive(false);
    //            sconeText.text = ""; }
    //        else { lightString = ""; lights1.SetActive(false); lights2.SetActive(false); lights3.SetActive(false); lights4.SetActive(false); FeedbackBanner.Instance.Show("Hmm... Let's try that again."); sconeText.text = ""; }    //reset
    //    }
    //}

    private IEnumerator successdelay()
    {
        audioSource.PlayOneShot(sizzleFire);
        yield return new WaitForSeconds(.5f);
        audioSource.PlayOneShot(fireSuccess);
    }
}