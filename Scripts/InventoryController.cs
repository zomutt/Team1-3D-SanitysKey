using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    public float flLife;
    public bool flWarningTextShown;
    public bool flOutTextShown;
    public TextMeshProUGUI flLifeDisplay;
    public TextMeshProUGUI medChargeDisplay; 
    public TextMeshProUGUI bulbChargeDisplay;
    public TextMeshProUGUI laudanumChargeDisplay;
    public GameObject flDirectionalLight;
    public int medpackHeal = 30;

    public PlayerController PlayerController;


    private void Awake()
    {
        flashlightOn = false;
        flDirectionalLight.SetActive(false);
        flWarningTextShown = false;
        flOutTextShown = false;
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
        }
    }

    private void Update()
    {
        string medChText = medCharges.ToString("F0");      //f = fixed-point format (reg decimal number), 0 = amt of decimal places to show,, kinda irrelevant here but i was sleep deprived. good practice for this at least. :) 
        string flLifeText = flLife.ToString("F0");
        string bulbChargeText = bulbCharges.ToString("F0");
        string laudinumChargeText = laudanumCharges.ToString("F0");
        medChargeDisplay.text = "(" + medChText + ")";
        flLifeDisplay.text = flLifeText + "%";
        bulbChargeDisplay.text = "(" + bulbChargeText + ")";
        laudanumChargeDisplay.text = "(" + laudinumChargeText + ")";
        if (flLife < 0) flLife = 0;
        if (flLife >= 100) flLife = 100;

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
                flWarningTextShown = true;
            }
            flWarningTextShown = true;
        }
        else if (flLife < 1)    //turns off fl when life = 0,, keep value at one bc the time operates in decimals even if doesnt show
        {
            FeedbackBanner.Instance.Show("Damn... I better find a light source.");
            flDirectionalLight.SetActive(false);
            if (!flOutTextShown)
            {
                FeedbackBanner.Instance.Show("Damn... I need to find a light source fast.");
                flWarningTextShown = true;
            }
        }

        if (Input.GetKeyDown("1"))
        {
            if (flLife < 1)
            {
                FeedbackBanner.Instance.Show("The bulb on this is out.");
            }
            else if (!flashlightOn && flLife > 1)   // turn ON
            {
                flDirectionalLight.SetActive(true);
                flashlightOn = true;
                Debug.Log("Turning ON FL");
            }
            else if (flashlightOn)                  // turn OFF
            {
                flDirectionalLight.SetActive(false);
                flashlightOn = false;
                Debug.Log("Turning OFF FL");
            }
        }

        if (Input.GetKeyDown("2"))
        {
            if (PlayerController.pHP >= 100)
            {
                FeedbackBanner.Instance.Show("I don't need to use that yet.");
            }
            else if ((PlayerController.pHP < 100 && medCharges > 0))
            {
                PlayerController.pHP += medpackHeal;
                Debug.Log("Player healed for: " + medpackHeal + " new HP: " + PlayerController.pHP);
                FeedbackBanner.Instance.Show("Ah, that's much better.");
                medCharges -= 1;
            }
            else if (medCharges < 1)
            {
                FeedbackBanner.Instance.Show("I need to find more medpacks.");
            }
        }

        if (Input.GetKeyDown("3"))
        {
            if (bulbCharges < 1)
            {
                FeedbackBanner.Instance.Show("I need to find more lightbulbs.");
            }
            else
            {
                flLife += bulbRestore;
            }
        }
        if (Input.GetKeyDown("4"))
        {
            if (laudanumCharges < 1)
            {
                FeedbackBanner.Instance.Show("I need to find more pills.");
            }
            else if (laudanumCharges > 0 && PlayerController.pSanity > 99)
            {
                FeedbackBanner.Instance.Show("I don't need to use that yet.");
            }
            else if (laudanumCharges > 0 && PlayerController.pSanity < 99)
            {
                FeedbackBanner.Instance.Show("Ah, that's much better.");
                PlayerController.pSanity += laudanumRestore;
            }
        }
    }
}
    