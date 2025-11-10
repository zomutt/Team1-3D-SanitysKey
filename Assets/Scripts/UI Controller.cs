using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text stamText;
    public TMP_Text sanityText;
    public PlayerController PlayerController;
    public float pHP, pStam, pSanity;
    string string_hp;
    string string_stam;
    string string_sanity;

    void Start()
    {
        if (PlayerController == null)
            PlayerController = Object.FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {                                      //some tedious ass shit i coulda probably done more efficiently but fuck it we ball
        pHP = PlayerController.pHP;
        pStam = PlayerController.pStam;
        pSanity = PlayerController.pSanity;
        
        string_hp = pHP.ToString();                 
        string_stam = pStam.ToString();
        string_sanity = pSanity.ToString();


       // healthText.text = "Health: " + string_hp;
        healthText.text = "Health: " + PlayerController.pHP.ToString("F0");         //gets rid of the decimals
        //stamText.text = "Stamina: " + string_stam;
        stamText.text = "Stamina: " + PlayerController.pStam.ToString("F0");
        //sanityText.text = "Sanity :" + string_sanity;
        sanityText.text = "Sanity :" + PlayerController.pSanity.ToString("F0");
    }
}
