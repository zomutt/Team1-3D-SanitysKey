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
    public float flLife;
    public float flDrain;
    public TextMeshProUGUI flLifeDisplay;
    public TextMeshProUGUI medChargeDisplay; 
    public GameObject flDirectionalLight;
    public int medpackHeal = 30;

    public PlayerController PlayerController;


    private void Awake()
    {
        flashlightOn = false;
        flDirectionalLight.SetActive(false);
        flLife = 100; //starts player with full charge bc we're nice :)
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            medCharges = 2;   //test purposes,, ensures we have enough to test stuff without giving them to us on other levels. i want players to have to find them.
        }
    }

    private void Update()
    {
        string medChText = medCharges.ToString("F0");      //f = fixed-point format (reg decimal number), 0 = amt of decimal places to show :) 
        string flLifeText = flLife.ToString("F0");
        medChargeDisplay.text = "(" + medChText + ")";
        flLifeDisplay.text = flLifeText + "%";
        while (flashlightOn) 
        {
            flLife -= flDrain * Time.deltaTime / 2;   
        }
        if (Input.GetKeyDown("1"))
        {
            if (flLife == 0)
            {
                FeedbackBanner.Instance.Show("I need another lightbulb.");
            }
            else if (!flashlightOn && flLife > 0)   // turn ON
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
            else
            {
                PlayerController.pHP += medpackHeal;
                Debug.Log("Player healed for: " + medpackHeal + " new HP: " + PlayerController.pHP);
                FeedbackBanner.Instance.Show("Ah, that's much better.");
                medCharges -= 1;
            }
        }
    }
}