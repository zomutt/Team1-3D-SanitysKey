using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Collections;

public class UIController : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text stamText;
    public TMP_Text sanityText;
    public PlayerController PlayerController;
    public float pHP, pStam, pSanity, flLife;
    string string_hp;
    string string_stam;
    string string_sanity;
    string string_fl;
    public Image FeedbackBG;
    public TextMeshProUGUI flLifeText;
    public InventoryController InventoryController;
    public GameObject SanityOverlay;
    public GameObject HurtOverlay;
    public GameObject player;
    public GameObject canteenInv;
    private void Awake()
    {
        player.SetActive(true);    //father forgive me for i have sinned
    }
    void Start()
    {
        if (PlayerController == null)
            PlayerController = Object.FindAnyObjectByType<PlayerController>();
        SanityOverlay.SetActive(true);
        HurtOverlay.SetActive(true);       //this is so that we dont have to constantly see these while editing
    }

    void Update()
    {                                      
        pHP = PlayerController.pHP;
        pStam = PlayerController.pStam;
        pSanity = PlayerController.pSanity;
        flLife = InventoryController.flLife;

        string_hp = pHP.ToString();                 
        string_stam = pStam.ToString();
        string_sanity = pSanity.ToString();
        string_fl = flLife.ToString();

        healthText.text = PlayerController.pHP.ToString("F0") + "%";         //gets rid of the decimals,, whole num only for display while letting everything function as a float. i am big brain.
        stamText.text = PlayerController.pStam.ToString("F0") + "%";  //f = fixed-point format (reg decimal number), 0 = amt of decimal places to show :) 
        sanityText.text = PlayerController.pSanity.ToString("F0") + "%";
        flLifeText.text = InventoryController.flLife.ToString("F0") + "%";


        if (InventoryController.canteenCount == 0) { canteenInv.SetActive(false); }
        else { canteenInv.SetActive(false); }
    }
}
